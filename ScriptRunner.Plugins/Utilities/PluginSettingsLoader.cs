using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for loading and merging plugin settings.
/// </summary>
public static class PluginSettingsLoader
{
    /// <summary>
    ///     Loads the plugin settings schema from a JSON file.
    /// </summary>
    /// <param name="pluginPath">The directory path where the plugin is located.</param>
    /// <returns>
    ///     An array of <see cref="PluginSettingDefinition" /> objects representing the plugin's settings schema.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the <c>plugin.settings.json</c> file is not found in the specified directory.
    /// </exception>
    /// <remarks>
    ///     This method expects a file named <c>plugin.settings.json</c> to exist in the provided directory.
    ///     The file should contain a JSON array of settings definitions.
    /// </remarks>
    public static PluginSettingDefinition[] LoadSettings(string pluginPath)
    {
        var settingsPath = Path.Combine(pluginPath, "plugin.settings.json");
        if (!File.Exists(settingsPath))
            throw new FileNotFoundException($"Settings file not found at: {settingsPath}");

        var jsonContent = File.ReadAllText(settingsPath);
        var settings = JsonConvert.DeserializeObject<PluginSettingDefinition[]>(jsonContent) ?? [];

        // Filter out invalid entries
        var validSettings = settings.Where(IsValidPluginSettingDefinition).ToArray();

        // Log any invalid entries for debugging
        var invalidSettings = settings.Except(validSettings).ToList();
        
        if (invalidSettings.Count == 0) return validSettings;
        
        foreach (var invalidSetting in invalidSettings)
        {
            Console.WriteLine($"Invalid plugin setting detected: {JsonConvert.SerializeObject(invalidSetting)}");
        }

        return validSettings;
    }

    /// <summary>
    /// Merges two collections of plugin settings and returns a unified collection of
    /// <see cref="PluginSettingDefinition" />.
    /// </summary>
    /// <param name="schema">The settings schema loaded from the plugin's <c>plugin.settings.json</c> file.</param>
    /// <param name="userValues">A collection of <see cref="PluginSettingDefinition" /> representing user-defined settings.</param>
    /// <returns>
    /// A merged collection of <see cref="PluginSettingDefinition" /> where user-defined values override schema defaults.
    /// </returns>
    public static IEnumerable<PluginSettingDefinition> MergeSettings(
        IEnumerable<PluginSettingDefinition> schema,
        IEnumerable<PluginSettingDefinition>? userValues)
    {
        // Create a dictionary from userValues to look up user-defined settings by key.
        var userDictionary = userValues?
                                 .GroupBy(setting => setting.Key)
                                 .ToDictionary(group => group.Key, group => group.First())
                             ?? new Dictionary<string, PluginSettingDefinition>();

        // Merge schema and user values
        var mergedSettings = schema
            .GroupBy(schemaSetting => schemaSetting.Key)
            .Select(group =>
            {
                var schemaSetting = group.First();

                // Check if a user-defined value exists for this key
                if (userDictionary.TryGetValue(schemaSetting.Key, out var userSetting))
                {
                    // Return a new PluginSettingDefinition with user-defined value overriding the schema
                    return new PluginSettingDefinition
                    {
                        Key = schemaSetting.Key,
                        Type = schemaSetting.Type,
                        Value = userSetting.Value ?? schemaSetting.Value
                    };
                }

                // If no user-defined value exists, return the schema setting as is
                return schemaSetting;
            })
            .ToList();

        return mergedSettings;
    }
    
    /// <summary>
    /// Validates a <see cref="PluginSettingDefinition"/> object.
    /// </summary>
    /// <param name="setting">The setting to validate.</param>
    /// <returns>True if the setting is valid; otherwise, false.</returns>
    private static bool IsValidPluginSettingDefinition(PluginSettingDefinition setting)
    {
        return !string.IsNullOrWhiteSpace(setting.Key) &&
               !string.IsNullOrWhiteSpace(setting.Type);
    }
}