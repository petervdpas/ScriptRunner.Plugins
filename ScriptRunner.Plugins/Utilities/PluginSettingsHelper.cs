using System;
using System.Collections.Generic;
using System.Text.Json;
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

    /// <summary>
    /// Initializes the <see cref="ILocalStorage" /> instance for the plugin.
    /// </summary>
    /// <param name="localStorage">The local storage instance to set.</param>
    public static void InitializeLocalStorage(ILocalStorage localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }
    
    /// <summary>
    /// Returns the <see cref="ILocalStorage" /> instance of the plugin.
    /// </summary>
    public static ILocalStorage FetchLocalStorage()
    {
        return _localStorage ?? throw new InvalidOperationException("LocalStorage has not been initialized.");
    }
    
    /// <summary>
    /// Stores plugin settings into <see cref="ILocalStorage" />.
    /// </summary>
    /// <param name="settings">The settings to store.</param>
    public static void StoreSettings(IEnumerable<PluginSettingDefinition> settings)
    {
        if (_localStorage == null)
            throw new InvalidOperationException("LocalStorage has not been initialized.");

        ArgumentNullException.ThrowIfNull(settings, nameof(settings));

        foreach (var setting in settings)
        {
            setting.Value = setting.Value switch
            {
                null => throw new ArgumentNullException(nameof(settings),
                    $"The value for setting with key '{setting.Key}' cannot be null."),
                string stringValue => stringValue.Trim('"'),
                _ => setting.Value
            };

            _localStorage.SetData(setting.Key, setting.Value);
        }
    }

    /// <summary>
    /// Retrieves a plugin setting value from <see cref="ILocalStorage" />.
    /// </summary>
    /// <typeparam name="T">The expected type of the setting value.</typeparam>
    /// <param name="key">The key of the setting to retrieve.</param>
    /// <returns>The value of the setting, or default if not found.</returns>
    public static T? RetrieveSetting<T>(string key)
    {
        if (_localStorage == null)
            throw new InvalidOperationException("LocalStorage has not been initialized.");

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        try
        {
            var value = _localStorage.GetData<object>(key);

            switch (value)
            {
                case null:
                    return default;
                case string stringValue:
                    stringValue = stringValue.Trim('"');

                    // Handle conversion to int
                    if (typeof(T) == typeof(int) && int.TryParse(stringValue, out var intValue))
                        return (T)(object)intValue;

                    // Handle conversion to bool
                    if (typeof(T) == typeof(bool) && bool.TryParse(stringValue, out var boolValue))
                        return (T)(object)boolValue;

                    // Handle conversion to float
                    if (typeof(T) == typeof(float) && float.TryParse(stringValue, out var floatValue))
                        return (T)(object)floatValue;

                    // Handle conversion to double
                    if (typeof(T) == typeof(double) && double.TryParse(stringValue, out var doubleValue))
                        return (T)(object)doubleValue;

                    // Return as string if T is string
                    if (typeof(T) == typeof(string))
                        return (T)(object)stringValue;

                    break;
            }

            // Direct cast if value is already of the desired type
            if (value is T typedValue)
                return typedValue;

            // Attempt conversion for other types
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException)
        {
            Console.WriteLine($"Error retrieving setting: {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Displays all plugin settings stored in <see cref="ILocalStorage" />.
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
            // Handle special formatting for complex objects
            var displayValue = value switch
            {
                null => "null",
                JsonElement jsonElement => jsonElement.ToString(),
                _ => value.ToString()
            };

            Console.WriteLine($"- Key: {key}, Value: {displayValue}");
        }
    }
}
