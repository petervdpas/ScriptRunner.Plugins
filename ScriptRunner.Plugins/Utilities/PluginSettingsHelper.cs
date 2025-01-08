using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for working with <see cref="PluginSettingDefinition" /> instances and integrates
///     with <see cref="ILocalStorage" /> for dynamic storage.
/// </summary>
public static class PluginSettingsHelper
{
    /// <summary>
    /// Stores plugin settings into <see cref="ILocalStorage" />.
    /// </summary>
    /// <param name="localStorage">The local storage instance for the plugin.</param>
    /// <param name="settings">The settings to store.</param>
    public static void StoreSettings(ILocalStorage localStorage, IEnumerable<PluginSettingDefinition> settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        foreach (var setting in settings)
        {
            if (setting.Value is null)
            {
                throw new ArgumentNullException(nameof(settings), 
                    $"The value for setting with key '{setting.Key}' cannot be null.");
            }

            localStorage.SetData(setting.Key, setting.Value);
        }
    }

    /// <summary>
    /// Retrieves a plugin setting value from <see cref="ILocalStorage" />.
    /// </summary>
    /// <typeparam name="T">The expected type of the setting value.</typeparam>
    /// <param name="localStorage">The local storage instance for the plugin.</param>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <returns>The value of the setting, or default if not found.</returns>
    public static T? RetrieveSetting<T>(ILocalStorage localStorage, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        try
        {
            return localStorage.GetData<T>(key);
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"Error retrieving setting: {ex.Message}");
            return default; // Handle gracefully or log
        }
    }

    /// <summary>
    ///     Displays all plugin settings stored in <see cref="ILocalStorage" />.
    /// </summary>
    /// <param name="localStorage">The local storage instance for the plugin.</param>
    public static void DisplayStoredSettings(ILocalStorage localStorage)
    {
        var data = localStorage.GetStorage();
        if (data.Count == 0)
        {
            Console.WriteLine("No settings found in local storage.");
            return;
        }

        Console.WriteLine("Stored plugin settings:");
        foreach (var (key, value) in data)
        {
            Console.WriteLine($"- Key: {key}, Value: {value}");
        }
    }
}
