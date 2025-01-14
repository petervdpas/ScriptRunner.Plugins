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
    public static PluginSettingDefinition[] LoadSettings(string pluginPath, bool showLogging = false)
    {
        var settingsPath = Path.Combine(pluginPath, "plugin.settings.json");

        if (!File.Exists(settingsPath))
        {
            if (showLogging)
            {
                Console.WriteLine($"Settings file not found: {settingsPath}");
            }
            return [];
        }

        var jsonContent = File.ReadAllText(settingsPath);

        try
        {
            // Deserialize JSON using SerializationHelper
            var rawSettings = SerializationHelper.Deserialize<RawPluginSettingDefinition[]>(jsonContent);

            if (rawSettings == null || rawSettings.Length == 0)
            {
                if (showLogging)
                {
                    Console.WriteLine("No valid plugin settings were found in the file.");
                }
                return [];
            }

            // Map to PluginSettingDefinition and validate types
            var settings = rawSettings.Select(s =>
            {
                var value = SerializationHelper.Deserialize<object>(s.DefaultValue?.ToString() ?? string.Empty);

                if (IsValueCompatibleWithType(value, s.Type))
                {
                    return new PluginSettingDefinition
                    {
                        Key = s.Key,
                        Type = s.Type,
                        Value = value
                    };
                }

                if (showLogging)
                {
                    Console.WriteLine($"Warning: Default value for key '{s.Key}' is not compatible with type '{s.Type}'. Setting it to null.");
                }

                return new PluginSettingDefinition
                {
                    Key = s.Key,
                    Type = s.Type,
                    Value = null
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
        var userDictionary = userValues?
                                 .GroupBy(setting => setting.Key)
                                 .ToDictionary(
                                     group => group.Key, 
                                     group => group.First())
                             ?? new Dictionary<string, PluginSettingDefinition>();

        return schema.Select(schemaSetting =>
        {
            if (userDictionary.TryGetValue(schemaSetting.Key, out var userSetting))
            {
                return new PluginSettingDefinition
                {
                    Key = schemaSetting.Key,
                    Type = schemaSetting.Type,
                    Value = userSetting.Value ?? schemaSetting.Value
                };
            }

            return schemaSetting;
        }).ToList();
    }

    /// <summary>
    /// Validates if a value is compatible with the specified type.
    /// </summary>
    private static bool IsValueCompatibleWithType(object? value, string type)
    {
        if (value == null) return false;

        return type.ToLowerInvariant() switch
        {
            "string" => value is string,
            "int" => int.TryParse(value.ToString(), out _),
            "bool" => bool.TryParse(value.ToString(), out _),
            "double" => double.TryParse(value.ToString(), out _),
            "float" => float.TryParse(value.ToString(), out _),
            _ => true
        };
    }
}