using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for working with <see cref="PluginSettingDefinition" /> instances.
/// </summary>
/// <remarks>
///     The <see cref="PluginSettingsHelper" /> class includes methods for displaying, manipulating,
///     and interacting with <see cref="PluginSettingDefinition" /> collections. These methods are designed to
///     simplify common tasks such as inspecting key-value pairs or validating the contents of
///     plugin settings.
/// </remarks>
public static class PluginSettingsHelper
{
    /// <summary>
    ///     Displays all <see cref="PluginSettingDefinition" /> instances in a user-friendly format.
    /// </summary>
    /// <param name="settings">The collection of <see cref="PluginSettingDefinition" /> to display. Can be <c>null</c>.</param>
    /// <remarks>
    ///     This method handles the following scenarios:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>If the <paramref name="settings" /> is <c>null</c>, it outputs a null message.</description>
    ///         </item>
    ///         <item>
    ///             <description>If the <paramref name="settings" /> is empty, it outputs an empty message.</description>
    ///         </item>
    ///         <item>
    ///             <description>If the <paramref name="settings" /> contains values, it lists all key-type-value triplets.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static void DisplayValues(IEnumerable<PluginSettingDefinition>? settings)
    {
        if (settings == null)
        {
            Console.WriteLine("The provided settings collection is null.");
            return;
        }

        var settingsList = new List<PluginSettingDefinition>(settings);
        if (settingsList.Count == 0)
        {
            Console.WriteLine("The settings collection is empty.");
            return;
        }

        Console.WriteLine("Plugin settings:");
        foreach (var setting in settingsList)
            Console.WriteLine($"- Key: {setting.Key}, Type: {setting.Type}, Value: {setting.Value ?? "null"}");
    }
}