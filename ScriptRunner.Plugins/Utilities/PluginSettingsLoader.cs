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
        if (!File.Exists(settingsPath)) throw new FileNotFoundException($"Settings file not found at: {settingsPath}");

        var jsonContent = File.ReadAllText(settingsPath);
        return JsonConvert.DeserializeObject<PluginSettingDefinition[]>(jsonContent) ?? [];
    }

    /// <summary>
    ///     Merges two collections of plugin settings and returns a unified collection of
    ///     <see cref="PluginSettingDefinition" />.
    /// </summary>
    /// <param name="schema">The settings schema loaded from the plugin's <c>plugin.settings.json</c> file.</param>
    /// <param name="userValues">A collection of <see cref="PluginSettingDefinition" /> representing user-defined settings.</param>
    /// <returns>
    ///     A merged collection of <see cref="PluginSettingDefinition" /> where user-defined values override schema defaults.
    /// </returns>
    /// <remarks>
    ///     This method ensures that the resulting settings collection contains all keys from the schema,
    ///     with user-defined values taking precedence.
    /// </remarks>
    public static IEnumerable<PluginSettingDefinition> MergeSettings(
        IEnumerable<PluginSettingDefinition> schema,
        IEnumerable<PluginSettingDefinition>? userValues)
    {
        var userDictionary = userValues?.ToDictionary(setting => setting.Key, setting => setting)
                             ?? new Dictionary<string, PluginSettingDefinition>();

        return schema.Select(schemaSetting =>
        {
            if (userDictionary.TryGetValue(schemaSetting.Key, out var userSetting))
                return new PluginSettingDefinition
                {
                    Key = schemaSetting.Key,
                    Type = schemaSetting.Type,
                    Value = userSetting.Value ?? schemaSetting.Value
                };

            return schemaSetting;
        }).ToList();
    }
}