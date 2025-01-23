using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Logging;
using ScriptRunner.Plugins.Models.DynamicClass;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Provides functionality to dynamically generate C# classes and compile them into an assembly based on JSON input.
///     This class allows for creating namespaces, classes, and properties as defined in a structured JSON format.
///     Implements <see cref="IDynamicClassGenerator" />.
/// </summary>
public class DynamicClassGenerator : IDynamicClassGenerator
{
    private readonly IPluginLogger? _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DynamicClassGenerator" /> class with an optional logger.
    /// </summary>
    /// <param name="logger">An optional logger for diagnostic and error logging.</param>
    public DynamicClassGenerator(IPluginLogger? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Generates an assembly from JSON definitions, saving it to the specified output path.
    /// </summary>
    /// <param name="json">
    ///     The JSON string that defines namespaces, classes, and properties for code generation.
    ///     Each entry in the JSON defines a namespace containing one or more classes, with each class specifying properties
    ///     and types.
    /// </param>
    /// <param name="outputDllPath">The file path where the generated DLL will be saved.</param>
    /// <returns>
    ///     A tuple containing:
    ///     <list type="bullet">
    ///         <item><term>AssemblyPath</term> - The path to the generated DLL if successful; otherwise, <c>null</c>.</item>
    ///         <item><term>Namespaces</term> - A list of namespaces defined within the generated assembly.</item>
    ///     </list>
    /// </returns>
    /// <remarks>
    ///     The JSON format must follow a structure that includes an array of namespace definitions. Each namespace can
    ///     contain multiple classes, and each class defines properties with types and optional using statements.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if the JSON input is improperly formatted or missing essential information.</exception>
    /// <exception cref="IOException">Thrown if the DLL could not be saved to the specified path.</exception>
    public (string? AssemblyPath, List<string> Namespaces) GenerateAssemblyFromJson(string json, string outputDllPath)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON content cannot be null or empty.", nameof(json));

        if (string.IsNullOrWhiteSpace(outputDllPath))
            throw new ArgumentException("Output DLL path cannot be null or empty.", nameof(outputDllPath));

        var rootDefinitions = ParseJsonToRootDefinitions(json);
        if (rootDefinitions.Count == 0)
        {
            _logger?.Warning("No valid definitions found in the provided JSON.");
            return (null, []);
        }

        var allNamespaces = new List<string>();
        var codeBuilder = new StringBuilder();

        // Collect unique usings across all definitions
        var uniqueUsings = new HashSet<string>(rootDefinitions.SelectMany(rd => rd.Usings));

        // Place all using directives at the top
        codeBuilder.AppendLine(string.Join(Environment.NewLine, uniqueUsings.Select(u => $"using {u};")));
        codeBuilder.AppendLine(); // Separate usings from namespaces

        // Process each root definition (namespace block)
        foreach (var rootDefinition in rootDefinitions)
        {
            var namespaceName = rootDefinition.Namespace;
            allNamespaces.Add(namespaceName);
            var classDefinitions = rootDefinition.Classes;

            // Generate code for each namespace block
            codeBuilder.Append(GenerateNamespaceSourceCode(classDefinitions, namespaceName));
        }

        var sourceCode = codeBuilder.ToString();

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        // Add the Microsoft.CSharp assembly
        references.Add(MetadataReference.CreateFromFile(
            typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location));

        var projectReferences = AssemblyHelper.GetProjectReferences(references).ToList();
        var utilitiesReferences = AssemblyHelper.LoadUtilitiesReferences(projectReferences).ToList();
        var netStandardReference = AssemblyHelper.GetNetStandardReference();

        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(outputDllPath))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithNullableContextOptions(NullableContextOptions.Enable))
            .AddReferences(netStandardReference)
            .AddReferences(references)
            .AddReferences(projectReferences)
            .AddReferences(utilitiesReferences)
            .AddSyntaxTrees(syntaxTree);

        using var dllStream = new FileStream(outputDllPath, FileMode.Create);
        var result = compilation.Emit(dllStream);

        if (result.Success)
        {
            _logger?.Information($"Assembly successfully generated at {outputDllPath}");
            return (outputDllPath, allNamespaces);
        }

        foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            _logger?.Error($"Compilation Error: {diagnostic}");

        return (null, []);
    }

    /// <summary>
    ///     Generates the C# source code for a specific namespace, including all classes, properties, and methods.
    /// </summary>
    /// <param name="classDefinitions">
    ///     A list of classes to generate within the specified namespace. Each class definition includes properties,
    ///     methods, and their respective types.
    /// </param>
    /// <param name="namespaceName">
    ///     The name of the namespace to define in the generated code. This namespace will contain all generated classes.
    /// </param>
    /// <returns>
    ///     A string containing the generated C# source code for the specified namespace, including definitions for
    ///     classes, properties, and methods as outlined in the class definitions.
    /// </returns>
    private static string GenerateNamespaceSourceCode(List<ClassDefinition> classDefinitions, string namespaceName)
    {
        var codeBuilder = new StringBuilder();

        codeBuilder.AppendLine($"namespace {namespaceName}");
        codeBuilder.AppendLine("{");

        foreach (var classDef in classDefinitions)
        {
            // Handle interface implementations
            var interfaces = classDef.Implements is { Count: > 0 }
                ? " : " + string.Join(", ", classDef.Implements)
                : string.Empty;

            codeBuilder.AppendLine($"    public class {classDef.Name}{interfaces}");
            codeBuilder.AppendLine("    {");

            foreach (var property in classDef.Properties) codeBuilder.AppendLine(GenerateProperty(property));

            foreach (var constructor in classDef.Constructors)
                codeBuilder.AppendLine(GenerateConstructor(classDef, constructor));

            foreach (var method in classDef.Methods) codeBuilder.AppendLine(GenerateMethod(method));

            codeBuilder.AppendLine("    }");
        }

        codeBuilder.AppendLine("}");
        return codeBuilder.ToString();
    }

    /// <summary>
    ///     Generates the C# code for a property definition.
    /// </summary>
    /// <param name="property">The property definition, including its name, type, accessor visibility, and accessor type.</param>
    /// <returns>A string containing the C# code for the property.</returns>
    /// <remarks>
    ///     This method constructs a C# property definition using the provided details, such as its type, name,
    ///     and accessor visibility (e.g., public/private) and accessor type (e.g., set/init).
    /// </remarks>
    private static string GenerateProperty(PropertyDefinition property)
    {
        var accessorVisibility = string.IsNullOrEmpty(property.AccessorVisibility)
            ? string.Empty
            : property.AccessorVisibility + " ";
        return
            $"        public {property.Type} {property.Name} {{ get; {accessorVisibility}{property.AccessorType}; }}";
    }

    /// <summary>
    ///     Generates the C# code for a class constructor based on its definition.
    /// </summary>
    /// <param name="classDef">The class definition to which the constructor belongs.</param>
    /// <param name="constructor">The constructor definition, including its parameters and body lines.</param>
    /// <returns>A string containing the C# code for the constructor.</returns>
    /// <remarks>
    ///     This method constructs a constructor definition, including its parameters and body. Each parameter is
    ///     represented as a type and name pair, and the body is a series of lines that define the constructor's logic.
    /// </remarks>
    private static string GenerateConstructor(ClassDefinition classDef, ConstructorDefinition constructor)
    {
        var parameters = string.Join(", ", constructor.Parameters.Select(p => $"{p.Type} {p.Name}"));
        var body = string.Join(Environment.NewLine, constructor.BodyLines.Select(line => $"            {line}"));

        return $@"
            public {classDef.Name}({parameters})
            {{
    {body}
            }}";
    }

    /// <summary>
    ///     Generates the C# code for a method definition.
    /// </summary>
    /// <param name="method">The method definition, including its name, return type, parameters, and body lines.</param>
    /// <returns>A string containing the C# code for the method.</returns>
    /// <remarks>
    ///     This method constructs a method definition using the provided details, such as its return type, name,
    ///     parameters, and body. The method body is composed of multiple lines of code.
    /// </remarks>
    private static string GenerateMethod(MethodDefinition method)
    {
        var parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
        var body = string.Join(Environment.NewLine, method.BodyLines.Select(line => $"            {line}"));

        return $@"
            public {method.ReturnType} {method.Name}({parameters})
            {{
    {body}
            }}";
    }

    /// <summary>
    ///     Parses a JSON string into a list of <see cref="RootDefinition" /> objects, representing namespaces and their
    ///     classes.
    /// </summary>
    /// <param name="json">The JSON string to parse. It should define namespaces, classes, and their properties.</param>
    /// <returns>
    ///     A list of <see cref="RootDefinition" /> objects representing the namespaces, classes, and properties in the JSON.
    ///     Returns an empty list if parsing fails or if the JSON is malformed.
    /// </returns>
    private static List<RootDefinition> ParseJsonToRootDefinitions(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<RootDefinition>>(json) ?? [];
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"JSON Parsing Error: {ex.Message}");
            throw new ArgumentException("The provided JSON is malformed.", nameof(json), ex);
        }
    }
}