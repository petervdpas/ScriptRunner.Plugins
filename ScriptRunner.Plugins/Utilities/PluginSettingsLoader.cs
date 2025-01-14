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
    /// <param name="showLogging">A flag to set logging on or off</param>
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
    public static PluginSettingDefinition[] LoadSettings(string pluginPath, bool showLogging = false)
    {
        if (string.IsNullOrWhiteSpace(pluginPath))
        {
            if (showLogging)
            {
                Console.WriteLine("Plugin path is null or empty.");
            }
            return [];
        }

        var settingsPath = Path.Combine(pluginPath, "plugin.settings.json");
        
        if (!File.Exists(settingsPath))
        {
            if (showLogging)
            {
                Console.WriteLine($"Settings file not found: {settingsPath}");
            }
            return [];
        }

        string jsonContent;
        try
        {
            jsonContent = File.ReadAllText(settingsPath);
        }
        catch (Exception ex)
        {
            if (showLogging)
            {
                Console.WriteLine($"Error reading settings file: {ex.Message}");
            }
            return [];
        }

        try
        {
            // Deserialize JSON
            var rawSettings = JsonConvert.DeserializeObject<RawPluginSettingDefinition[]>(jsonContent);

            if (rawSettings == null || rawSettings.Length == 0)
            {
                if (showLogging)
                {
                    Console.WriteLine("No valid plugin settings were found in the file.");
                }
                return [];
            }

            // Map to PluginSettingDefinition and validate default values
            var settings = rawSettings.Select(s =>
            {
                // Validate default value compatibility
                var value = s.DefaultValue;
                if (value != null && !IsValueCompatibleWithType(value, s.Type))
                {
                    if (showLogging)
                    {
                        Console.WriteLine($"Default value for key '{s.Key}' is incompatible with type '{s.Type}'. Using null.");
                    }
                    value = null;
                }

                return new PluginSettingDefinition
                {
                    Key = s.Key,
                    Type = s.Type,
                    Value = value
                };
                
            }).ToArray();

            return settings;
        }
        catch (JsonException ex)
        {
            if (showLogging)
            {
                Console.WriteLine($"Error parsing JSON in {settingsPath}: {ex.Message}");
            }
            return [];
        }
        catch (Exception ex)
        {
            if (showLogging)
            {
                Console.WriteLine($"Unexpected error loading settings from {settingsPath}: {ex.Message}");
            }
            return [];
        }
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
    /// Validates if a value is compatible with the specified type.
    /// </summary>
    private static bool IsValueCompatibleWithType(object value, string type)
    {
        return type.ToLowerInvariant() switch
        {
            "string" => value is string,
            "int" => int.TryParse(value.ToString(), out _),
            "bool" => bool.TryParse(value.ToString(), out _),
            _ => true // Assume compatibility for unknown types
        };
    }
}