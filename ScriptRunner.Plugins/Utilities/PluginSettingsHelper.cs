using System;
using System.Collections.Generic;
using System.Linq;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for working with <see cref="PluginSettingDefinition" /> instances and integrates
///     with <see cref="ILocalStorage" /> for dynamic storage.
/// </summary>
public static class PluginSettingsHelper
{
    private static ILocalStorage? _localStorage;
    private static IEnumerable<PluginSettingDefinition>? _schema;
    
    /// <summary>
    ///     Initializes the <see cref="ILocalStorage" /> instance for the plugin.
    /// </summary>
    /// <param name="localStorage">The local storage instance to set.</param>
    public static void InitializeLocalStorage(ILocalStorage? localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    /// <summary>
    ///     Returns the <see cref="ILocalStorage" /> instance of the plugin.
    /// </summary>
    public static ILocalStorage FetchLocalStorage()
    {
        return _localStorage ?? throw new InvalidOperationException("LocalStorage has not been initialized.");
    }

    /// <summary>
    ///     Stores plugin settings into <see cref="ILocalStorage" />.
    /// </summary>
    /// <param name="settings">The settings to store.</param>
    public static void StoreSettings(IEnumerable<PluginSettingDefinition> settings)
    {
        if (_localStorage == null)
            throw new InvalidOperationException("LocalStorage has not been initialized.");

        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        _schema = settings;
        
        foreach (var setting in _schema)
        {
            if (setting.Value == null)
                throw new ArgumentNullException(nameof(settings),
                    $"The value for setting with key '{setting.Key}' cannot be null.");

            var serializedValue = SerializationHelper.Serialize(setting.Value);
            
            _localStorage.SetData(setting.Key, serializedValue);
        }
    }

    /// <summary>
    ///     Retrieves a plugin setting value from <see cref="ILocalStorage" />.
    /// </summary>
    /// <typeparam name="T">The expected type of the setting value.</typeparam>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <param name="withTrim">bool to remove quotes</param>
    /// <returns>The value of the setting, or default if not found.</returns>
    public static T? RetrieveSetting<T>(string key, bool withTrim = false)
    {
        if (_localStorage == null)
            throw new InvalidOperationException("LocalStorage has not been initialized.");

        var serializedValue = withTrim 
            ? _localStorage.GetData<string>(key)?.Trim('"') 
            : _localStorage.GetData<string>(key);

        if (string.IsNullOrEmpty(serializedValue)) return default;

        try
        {
            return SerializationHelper.Deserialize<T>(serializedValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing setting for key '{key}': {ex.Message}");
            return default;
        }
    }

    /// <summary>
    ///     Displays all plugin settings stored in <see cref="ILocalStorage" />.
    /// </summary>
    public static void DisplayStoredSettings()
    {
        if (_localStorage == null)
            throw new InvalidOperationException("LocalStorage has not been initialized.");

        var data = _localStorage.GetStorage();
        if (data.Count == 0)
        {
            Console.WriteLine("No settings found in local storage.");
            return;
        }

        Console.WriteLine("Stored plugin settings:");
        foreach (var (key, value) in data)
        {
            var displayValue = IsSecretSetting(key) ? "*****" : value;
            Console.WriteLine($"- Key: {key}, Value: {displayValue}");
        }
    }
    
    /// <summary>
    ///     Determines if the setting with the specified key is secret.
    /// </summary>
    private static bool IsSecretSetting(string key)
    {
        if (_schema == null)
            throw new InvalidOperationException("Settings schema has not been initialized.");

        var setting = _schema.FirstOrDefault(s => s.Key == key);
        return setting?.IsSecret ?? false;
    }
}