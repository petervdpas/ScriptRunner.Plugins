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
        var rootDefinitions = ParseJsonToRootDefinitions(json);
        if (rootDefinitions.Count == 0)
            return (null, []);

        var allNamespaces = new List<string>();
        var codeBuilder = new StringBuilder();

        // Collect unique usings across all definitions
        var uniqueUsings = new HashSet<string>(rootDefinitions.SelectMany(rd => rd.Usings));

        // Place all using directives at the top
        foreach (var ns in uniqueUsings) codeBuilder.AppendLine($"using {ns};");

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
        // Log.Debug(sourceCode);

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var existingReferences = references.ToList();
        var projectReferences = AssemblyHelper.GetProjectReferences(existingReferences).ToList();
        var utilitiesReferences = AssemblyHelper.LoadUtilitiesReferences(projectReferences).ToList();
        var netStandardReference = AssemblyHelper.GetNetStandardReference();

        var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(outputDllPath))
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(netStandardReference)
            .AddReferences(existingReferences)
            .AddReferences(projectReferences)
            .AddReferences(utilitiesReferences)
            .AddSyntaxTrees(syntaxTree);

        using var dllStream = new FileStream(outputDllPath, FileMode.Create);
        var result = compilation.Emit(dllStream);

        if (result.Success) return (outputDllPath, allNamespaces);

        foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            _logger?.Error(diagnostic.ToString());

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

            // Add properties
            foreach (var property in classDef.Properties)
            {
                var accessorType = property.AccessorType;
                var accessorVisibility = string.IsNullOrEmpty(property.AccessorVisibility)
                    ? string.Empty
                    : property.AccessorVisibility + " ";

                // Construct the property definition
                codeBuilder.AppendLine(
                    $"        public {property.Type} {property.Name} {{ get; {accessorVisibility}{accessorType}; }}");
            }

            // Constructors
            foreach (var constructor in classDef.Constructors)
            {
                codeBuilder.Append($"        public {classDef.Name}(");
                codeBuilder.Append(string.Join(", ", constructor.Parameters.Select(p => $"{p.Type} {p.Name}")));
                codeBuilder.AppendLine(")");

                codeBuilder.AppendLine("        {");

                // Join the BodyLines into individual lines of the constructor
                foreach (var line in constructor.BodyLines) codeBuilder.AppendLine($"            {line}");

                codeBuilder.AppendLine("        }");
            }

            // Methods
            foreach (var method in classDef.Methods)
            {
                codeBuilder.Append($"        public {method.ReturnType} {method.Name}(");
                codeBuilder.Append(string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}")));
                codeBuilder.AppendLine(")");

                codeBuilder.AppendLine("        {");

                // Join the BodyLines into individual lines of the method
                foreach (var line in method.BodyLines) codeBuilder.AppendLine($"            {line}");

                codeBuilder.AppendLine("        }");
            }

            codeBuilder.AppendLine("    }");
        }

        codeBuilder.AppendLine("}");
        return codeBuilder.ToString();
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
            return [];
        }
    }
}