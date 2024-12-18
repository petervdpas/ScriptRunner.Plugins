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
            // Use MetadataLoadContext to safely inspect assembly metadata
            var resolver = new PathAssemblyResolver(GetAssemblyPaths(fullPathToDll));
            using var metadataContext = new MetadataLoadContext(resolver);

            var assembly = metadataContext.LoadFromAssemblyPath(fullPathToDll);

            // Search for types that have the PluginMetadataAttribute
            var pluginType = assembly.GetTypes()
                .FirstOrDefault(t => t.GetCustomAttributes(typeof(PluginMetadataAttribute), false).Any());

            if (pluginType != null)
            {
                // Retrieve the PluginMetadataAttribute from the type
                var metadata = pluginType.GetCustomAttribute<PluginMetadataAttribute>();
                return metadata?.Name;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to inspect metadata in: {fullPathToDll}. {ex.Message}");
        }

        return null;
    }
    
    /// <summary>
    /// Retrieves all assembly paths needed for the MetadataLoadContext.
    /// </summary>
    private static string[] GetAssemblyPaths(string mainAssemblyPath)
    {
        var runtimePath = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        var assemblyDirectory = Path.GetDirectoryName(mainAssemblyPath) ?? string.Empty;

        return Directory.GetFiles(runtimePath, "*.dll")
            .Concat(Directory.GetFiles(assemblyDirectory, "*.dll"))
            .ToArray();
    }
}