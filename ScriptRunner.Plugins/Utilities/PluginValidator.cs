using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.Exceptions;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Default implementation of <see cref="IPluginValidator" /> for validating plugins.
/// </summary>
/// <remarks>
///     This class provides methods to validate plugin types to ensure they meet the required standards
///     for integration with the ScriptRunner framework.
/// </remarks>
public class PluginValidator : IPluginValidator
{
    /// <summary>
    /// Validates plugin metadata to ensure compatibility and required fields.
    /// </summary>
    /// <param name="metadata">The metadata associated with the plugin.</param>
    /// <exception cref="PluginInitializationException">
    /// Thrown if the plugin metadata is incompatible or missing required fields.
    /// </exception>
    public void ValidateMetadata(PluginMetadataAttribute metadata)
    {
        const string currentSystemVersion = PluginSystemConstants.CurrentPluginSystemVersion;
        const string currentFrameworkVersion = PluginSystemConstants.CurrentFrameworkVersion;

        if (string.IsNullOrWhiteSpace(metadata.Name))
            throw new PluginInitializationException("Plugin must have a non-empty Name property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.Version))
            throw new PluginInitializationException("Plugin must have a non-empty Version property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.PluginSystemVersion))
            throw new PluginInitializationException("Plugin must specify the PluginSystemVersion.");

        if (metadata.PluginSystemVersion != currentSystemVersion)
            throw new PluginInitializationException(
                $"Plugin is not compatible with the current plugin system version ({currentSystemVersion}). Required: {metadata.PluginSystemVersion}.");

        if (!string.IsNullOrWhiteSpace(metadata.FrameworkVersion) &&
            metadata.FrameworkVersion != currentFrameworkVersion)
            throw new PluginInitializationException(
                $"Plugin specifies a framework version ({metadata.FrameworkVersion}) that does not match the required version ({currentFrameworkVersion}).");
    }

    /// <summary>
    ///     Validates a plugin type to ensure it conforms to the required standards.
    /// </summary>
    /// <param name="pluginType">The type of the plugin to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginType" /> is null.</exception>
    /// <exception cref="PluginInitializationException">
    ///     Thrown if the plugin lacks required metadata, does not implement <see cref="IPlugin" />, or fails other validation
    ///     checks.
    /// </exception>
    public void Validate(Type pluginType)
    {
        if (pluginType == null)
            throw new ArgumentNullException(nameof(pluginType), "Plugin type cannot be null.");

        var metadata = pluginType.GetCustomAttribute<PluginMetadataAttribute>();
        if (metadata == null)
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} is missing the required PluginMetadataAttribute.");

        ValidateMetadata(metadata, pluginType);

        if (!typeof(IPlugin).IsAssignableFrom(pluginType))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must implement the IPlugin interface.");

        if (typeof(IServicePlugin).IsAssignableFrom(pluginType)) ValidateServicePlugin(pluginType, metadata);
    }

    /// <summary>
    ///     Validates an <see cref="ExpandoObject" /> against a plugin's JSON schema.
    /// </summary>
    /// <param name="configuration">The <see cref="ExpandoObject" /> to validate.</param>
    /// <param name="schemaPath">The file path to the JSON schema.</param>
    /// <exception cref="ArgumentException">Thrown if the validation fails.</exception>
    public void ValidateExpandoWithSchema(ExpandoObject configuration, string schemaPath)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null.");

        if (!File.Exists(schemaPath))
            throw new FileNotFoundException($"Schema file not found: {schemaPath}");

        var schemaJson = File.ReadAllText(schemaPath);
        var schema = JsonConvert.DeserializeObject<List<PluginSettingDefinition>>(schemaJson)
                     ?? throw new ArgumentException("Invalid schema format.");

        if (configuration is not IDictionary<string, object?> configDict)
            throw new ArgumentException("Configuration is not a valid ExpandoObject.");

        // Validate required keys and types
        foreach (var setting in schema)
        {
            if (!configDict.TryGetValue(setting.Key, out var value))
                throw new ArgumentException($"Missing required configuration key: {setting.Key}");

            if (value != null && !IsValidType(value, setting.Type))
                throw new ArgumentException(
                    $"Invalid type for key '{setting.Key}'. Expected {setting.Type}, but got {value?.GetType().Name ?? "null"}.");
        }

        // Validate for unexpected keys
        var schemaKeys = new HashSet<string>(schema.Select(s => s.Key));
        foreach (var key in configDict.Keys)
            if (!schemaKeys.Contains(key))
                throw new ArgumentException($"Unexpected configuration key: {key}");
    }

    /// <summary>
    ///     Validates the metadata attributes of a plugin to ensure required fields are provided.
    /// </summary>
    /// <param name="metadata">The metadata associated with the plugin.</param>
    /// <param name="pluginType">The type of the plugin being validated.</param>
    /// <exception cref="PluginInitializationException">
    ///     Thrown if required metadata fields (e.g., Name, Version, or PluginSystemVersion) are missing or invalid.
    /// </exception>
    private static void ValidateMetadata(PluginMetadataAttribute metadata, Type pluginType)
    {
        const string currentSystemVersion = PluginSystemConstants.CurrentPluginSystemVersion;
        const string currentFrameworkVersion = PluginSystemConstants.CurrentFrameworkVersion;

        if (string.IsNullOrWhiteSpace(metadata.Name))
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} must have a non-empty Name property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.Version))
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} must have a non-empty Version property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.PluginSystemVersion))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must specify the PluginSystemVersion.");

        if (metadata.PluginSystemVersion != currentSystemVersion)
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} is not compatible with the current plugin system version ({currentSystemVersion}). " +
                $"Required: {metadata.PluginSystemVersion}.");

        if (!string.IsNullOrWhiteSpace(metadata.FrameworkVersion) &&
            metadata.FrameworkVersion != currentFrameworkVersion)
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} specifies a framework version ({metadata.FrameworkVersion}) that does not match the required version ({currentFrameworkVersion}).");
    }

    /// <summary>
    ///     Validates the declared services in a plugin's metadata to ensure they are implemented.
    /// </summary>
    /// <param name="pluginType">The plugin type being validated.</param>
    /// <param name="metadata">The metadata associated with the plugin.</param>
    /// <exception cref="PluginInitializationException">
    ///     Thrown if declared services in the metadata are not implemented as methods in the plugin class.
    /// </exception>
    private static void ValidateServicePlugin(Type pluginType, PluginMetadataAttribute metadata)
    {
        if (metadata.Services == null || metadata.Services.Length == 0)
            return;

        var missingServices = metadata.Services.Where(service =>
            pluginType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .All(m => m.Name != service)).ToList();

        if (missingServices.Count != 0)
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} declares the following services in its metadata but does not implement them: {string.Join(", ", missingServices)}.");
    }

    /// <summary>
    ///     Determines if a value matches the expected type.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="expectedType">The expected type as a string.</param>
    /// <returns>True if the value matches the expected type; otherwise, false.</returns>
    private static bool IsValidType(object value, string expectedType)
    {
        return expectedType switch
        {
            "string" => value is string,
            "int" => value is int,
            "bool" => value is bool,
            "double" => value is double,
            _ => false
        };
    }
}