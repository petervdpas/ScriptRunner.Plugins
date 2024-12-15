using System;
using System.Collections.Generic;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for retrieving configuration values from a dictionary.
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    ///     Retrieves a configuration value from the provided dictionary.
    /// </summary>
    /// <typeparam name="T">The expected type of the configuration value.</typeparam>
    /// <param name="config">The dictionary containing configuration key-value pairs.</param>
    /// <param name="key">The key of the configuration value to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>
    ///     The configuration value associated with the key, cast to type <typeparamref name="T" />,
    ///     or the <paramref name="defaultValue" /> if the key is not found.
    /// </returns>
    public static T GetConfigValue<T>(IDictionary<string, object> config, string key, T defaultValue = default!)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config), "Configuration dictionary cannot be null.");

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Configuration key cannot be null or empty.", nameof(key));

        return config.TryGetValue(key, out var value) ? (T)value : defaultValue;
    }
}