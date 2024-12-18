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
            var resolver = new PathAssemblyResolver(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"));
            using var metadataContext = new MetadataLoadContext(resolver);

            var assembly = metadataContext.LoadFromAssemblyPath(fullPathToDll);

            // Iterate over all types in the assembly
            foreach (var type in assembly.GetTypes())
            {
                foreach (var attributeData in CustomAttributeData.GetCustomAttributes(type))
                {
                    if (attributeData.AttributeType.FullName != typeof(PluginMetadataAttribute).FullName) continue;
                    
                    // Retrieve the plugin name from the constructor arguments
                    var pluginNameArg = attributeData.ConstructorArguments.FirstOrDefault();
                    return pluginNameArg.Value?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to inspect metadata in: {fullPathToDll}. {ex.Message}");
        }

        return null;
    }
}