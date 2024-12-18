using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ScriptRunner.Plugins.Attributes;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods for inspecting assemblies to detect and retrieve plugin metadata.
/// </summary>
public static class PluginMetadataChecker
{
    /// <summary>
    /// Checks if the provided DLL contains a class with the <see cref="PluginMetadataAttribute"/> and retrieves the plugin name.
    /// </summary>
    /// <param name="fullPathToDll">The full path to the DLL file to inspect.</param>
    /// <returns>
    /// The plugin name as specified in the <see cref="PluginMetadataAttribute"/> if found; otherwise, <c>null</c>.
    /// </returns>
    public static string? GetPluginNameIfExists(string fullPathToDll)
    {
        if (!File.Exists(fullPathToDll))
            throw new FileNotFoundException($"DLL not found: {fullPathToDll}");

        try
        {
            // Use MetadataLoadContext for safe inspection
            var resolver = new PathAssemblyResolver(GetAllReferencedAssemblies(fullPathToDll));
            using var metadataLoadContext = new MetadataLoadContext(resolver);

            var assembly = metadataLoadContext.LoadFromAssemblyPath(fullPathToDll);

            // Search for types that have the PluginMetadataAttribute
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => t.GetCustomAttributes(typeof(PluginMetadataAttribute), false).Length != 0);

            if (pluginType != null)
            {
                // Retrieve the PluginMetadataAttribute from the type
                var metadata = pluginType.GetCustomAttribute<PluginMetadataAttribute>();
                return metadata?.Name; // Return the plugin name
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to inspect metadata in: {fullPathToDll}. {ex.Message}");
        }

        return null;
    }
    
    /// <summary>
    /// Retrieves all assemblies referenced by the specified DLL, including the core assembly.
    /// </summary>
    /// <param name="fullPathToDll">The path to the DLL.</param>
    /// <returns>Enumerable of assembly file paths.</returns>
    private static string[] GetAllReferencedAssemblies(string fullPathToDll)
    {
        var assemblyDirectory = Path.GetDirectoryName(fullPathToDll) ?? throw new InvalidOperationException("Invalid DLL path.");
        var coreAssemblyPath = typeof(object).Assembly.Location; // System.Private.CoreLib

        return Directory.EnumerateFiles(assemblyDirectory, "*.dll")
            .Append(coreAssemblyPath)
            .Distinct()
            .ToArray();
    }
}