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
    /// <exception cref="FileNotFoundException">
    /// Thrown when the specified DLL file does not exist.
    /// </exception>
    /// <remarks>
    /// This method dynamically loads the assembly and searches for types that are annotated with the
    /// <see cref="PluginMetadataAttribute"/>. If such a type is found, the plugin name is extracted from the attribute.
    /// </remarks>
    public static string? GetPluginNameIfExists(string fullPathToDll)
    {
        if (!File.Exists(fullPathToDll))
            throw new FileNotFoundException($"DLL not found: {fullPathToDll}");

        try
        {
            // Load the assembly from the specified path
            var assembly = Assembly.LoadFrom(fullPathToDll);

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
        catch (ReflectionTypeLoadException ex)
        {
            Console.WriteLine($"[ERROR] Could not load types from: {fullPathToDll}. {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to check metadata in: {fullPathToDll}. {ex.Message}");
        }

        // Return null if no metadata is found
        return null;
    }
}