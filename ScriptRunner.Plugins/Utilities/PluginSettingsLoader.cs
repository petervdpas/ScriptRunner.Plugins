using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods for loading and merging plugin settings.
/// </summary>
public static class PluginSettingsLoader
{
    /// <summary>
    /// Loads the plugin settings schema from a JSON file.
    /// </summary>
    /// <param name="pluginPath">The directory path where the plugin is located.</param>
    /// <returns>
    /// An array of <see cref="PluginSettingDefinition"/> objects representing the plugin's settings schema.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the <c>plugin.settings.json</c> file is not found in the specified directory.
    /// </exception>
    /// <remarks>
    /// This method expects a file named <c>plugin.settings.json</c> to exist in the provided directory.
    /// The file should contain a JSON array of settings definitions.
    /// </remarks>
    public static PluginSettingDefinition[] LoadSettings(string pluginPath)
    {
        var settingsPath = Path.Combine(pluginPath, "plugin.settings.json");
        if (!File.Exists(settingsPath))
        {
            throw new FileNotFoundException($"Settings file not found at: {settingsPath}");
        }

        var jsonContent = File.ReadAllText(settingsPath);
        return JsonConvert.DeserializeObject<PluginSettingDefinition[]>(jsonContent)
               ?? [];
    }
    
    /// <summary>
    /// Merges the plugin settings schema with user-defined values.
    /// </summary>
    /// <param name="schema">The settings schema loaded from the plugin's <c>plugin.settings.json</c> file.</param>
    /// <param name="userValues">An <see cref="ExpandoObject"/> containing user-defined settings values.</param>
    /// <returns>
    /// An <see cref="ExpandoObject"/> containing the merged settings, where user-defined values override
    /// schema defaults if present.
    /// </returns>
    /// <remarks>
    /// This method iterates over the settings schema and checks if each key exists in the user-defined values.
    /// If a key is present in <paramref name="userValues"/>, its value is used; otherwise, the default value
    /// from the schema is applied.
    /// </remarks>
    public static ExpandoObject MergeSettings(
        PluginSettingDefinition[] schema,
        ExpandoObject? userValues)
    {
        var mergedSettings = new ExpandoObject();
        IDictionary<string, object?> mergedDictionary = mergedSettings;

        // Safely cast userValues to a dictionary for easier access
        IDictionary<string, object?>? userDictionary = userValues;

        foreach (var setting in schema)
        {
            // Use user-defined value if available; otherwise, fall back to default
            if (userDictionary != null && userDictionary.TryGetValue(setting.Key, out var value))
            {
                mergedDictionary[setting.Key] = value;
            }
            else
            {
                mergedDictionary[setting.Key] = setting.DefaultValue;
            }
        }

        if (userDictionary == null) return mergedSettings;
        
        var schemaKeys = new HashSet<string>(schema.Select(s => s.Key));
        foreach (var userKey in userDictionary.Keys)
        {
            if (!schemaKeys.Contains(userKey))
            {
                throw new ArgumentException($"Unexpected key '{userKey}' found in user-defined settings.");
            }
        }

        return mergedSettings;
    }
}