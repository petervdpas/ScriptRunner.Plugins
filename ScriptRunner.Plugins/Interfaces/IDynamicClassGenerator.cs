using System;
using System.Collections.Generic;
using System.IO;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Represents a service for dynamically generating C# classes and compiling them into an assembly from JSON definitions.
/// </summary>
public interface IDynamicClassGenerator
{
    /// <summary>
    /// Generates an assembly from JSON definitions and saves it to the specified output path.
    /// </summary>
    /// <param name="json">
    /// A JSON string that defines the structure of namespaces, classes, properties, and methods to generate.
    /// Each entry in the JSON should specify a namespace, containing classes with properties, methods, and constructors.
    /// </param>
    /// <param name="outputDllPath">
    /// The file path where the compiled DLL will be saved. This should be a valid writable file path.
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///     <item>
    ///         <term>AssemblyPath</term>
    ///         <description>The path to the generated DLL if compilation succeeds; otherwise, <c>null</c>.</description>
    ///     </item>
    ///     <item>
    ///         <term>Namespaces</term>
    ///         <description>A list of namespaces defined within the generated assembly.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the JSON input is invalid or does not adhere to the required structure.
    /// </exception>
    /// <exception cref="IOException">
    /// Thrown if the output DLL file cannot be created or written to.
    /// </exception>
    (string? AssemblyPath, List<string> Namespaces) GenerateAssemblyFromJson(string json, string outputDllPath);
}