using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Provides utility methods to retrieve metadata references for assemblies used by the current project.
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    ///     Retrieves a collection of <see cref="MetadataReference" /> objects for assemblies not already in the provided
    ///     references.
    /// </summary>
    /// <param name="existingReferences">
    ///     An optional collection of <see cref="MetadataReference" /> objects to exclude from the references loaded from the
    ///     <c>ScriptRunner.Utilities</c> project.
    ///     If not provided, all referenced assemblies are loaded.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> containing <see cref="MetadataReference" /> objects for each non-dynamic assembly
    ///     that is not in the <paramref name="existingReferences" /> list.
    /// </returns>
    public static IEnumerable<MetadataReference> GetProjectReferences(
        IEnumerable<MetadataReference>? existingReferences = null)
    {
        var utilitiesAssembly = typeof(AssemblyHelper).Assembly;
        var referencedAssemblies = utilitiesAssembly.GetReferencedAssemblies();

        // Extract the names of the already loaded assemblies
        var loadedAssemblyNames = existingReferences?
            .Where(r => !string.IsNullOrEmpty(r.Display)) // Ensure Display is not null or empty
            .Select(r => AssemblyName.GetAssemblyName(r.Display!).Name)
            .ToHashSet() ?? [];

        // Exclude assemblies that are already in the existingReferences list
        referencedAssemblies = referencedAssemblies
            .Where(a => !loadedAssemblyNames.Contains(a.Name))
            .ToArray();

        return referencedAssemblies
            .Select(Assembly.Load)
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));
    }

    /// <summary>
    ///     Loads metadata references for all assemblies referenced by the <c>ScriptRunner.Utilities</c> assembly,
    ///     and includes additional required references, excluding those that are already available in
    ///     <paramref name="existingReferences" />.
    /// </summary>
    /// <param name="existingReferences">
    ///     A collection of <see cref="MetadataReference" /> objects representing already loaded assembly references to
    ///     exclude.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{MetadataReference}" /> containing <see cref="MetadataReference" /> objects for each
    ///     successfully loaded assembly reference.
    /// </returns>
    public static IEnumerable<MetadataReference> LoadUtilitiesReferences(
        IEnumerable<MetadataReference>? existingReferences = null)
    {
        var utilitiesAssembly = typeof(AssemblyHelper).Assembly;
        var referencedAssemblies = utilitiesAssembly.GetReferencedAssemblies();

        // Extract the names of the already loaded assemblies
        var loadedAssemblyNames = existingReferences?
            .Where(r => !string.IsNullOrEmpty(r.Display)) // Ensure Display is not null or empty
            .Select(r => AssemblyName.GetAssemblyName(r.Display!).Name)
            .ToHashSet() ?? [];

        // Exclude assemblies that are already in the existingReferences list
        referencedAssemblies = referencedAssemblies
            .Where(a => !loadedAssemblyNames.Contains(a.Name))
            .ToArray();

        return referencedAssemblies
            .Select(Assembly.Load)
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location));
    }

    /// <summary>
    ///     Retrieves a metadata reference for the <c>netstandard.dll</c> assembly, which is required for projects targeting
    ///     .NET Standard.
    /// </summary>
    /// <returns>
    ///     A <see cref="MetadataReference" /> object representing the <c>netstandard.dll</c> assembly, used to resolve types
    ///     defined in .NET Standard libraries.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the <c>netstandard.dll</c> file is not found in the runtime directory.
    /// </exception>
    /// <remarks>
    ///     This method locates the <c>netstandard.dll</c> assembly in the runtime directory of the current environment
    ///     and creates a metadata reference from it. This reference is necessary to resolve compatibility issues
    ///     when working with .NET Standard libraries.
    /// </remarks>
    /// s
    public static MetadataReference GetNetStandardReference()
    {
        var netstandardPath = Path.Combine(
            RuntimeEnvironment.GetRuntimeDirectory(),
            "netstandard.dll");

        if (File.Exists(netstandardPath)) return MetadataReference.CreateFromFile(netstandardPath);

        throw new FileNotFoundException("Could not find netstandard.dll in the runtime directory.");
    }
    
    /// <summary>
    /// Checks if an assembly with the specified name is already loaded in the host context.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to check.</param>
    /// <returns>True if the assembly is already loaded, otherwise false.</returns>
    public static bool IsAssemblyLoaded(string assemblyName)
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        return loadedAssemblies.Any(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Attempts to get the location of an already loaded assembly.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to locate.</param>
    /// <returns>The file path of the assembly if loaded, otherwise null.</returns>
    public static string? GetLoadedAssemblyLocation(string assemblyName)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));

        return assembly?.Location;
    }
}