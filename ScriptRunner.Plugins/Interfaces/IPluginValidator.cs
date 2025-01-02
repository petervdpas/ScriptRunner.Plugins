using System;
using System.Dynamic;
using System.IO;
using ScriptRunner.Plugins.Exceptions;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a plugin validator that ensures plugins meet required standards for integration.
/// </summary>
public interface IPluginValidator
{
    /// <summary>
    ///     Validates a plugin type to ensure it conforms to required standards.
    /// </summary>
    /// <param name="pluginType">The type of the plugin to validate.</param>
    /// <exception cref="PluginInitializationException">
    ///     Thrown if the plugin lacks required metadata, is missing a name or version, or does not implement required
    ///     interfaces.
    /// </exception>
    void Validate(Type pluginType);

    /// <summary>
    /// Validates a provided <see cref="ExpandoObject" /> configuration against a plugin's schema definition.
    /// </summary>
    /// <param name="configuration">The configuration objects to validate.</param>
    /// <param name="schemaPath">
    /// The file path to the schema definition (e.g., <c>plugin.settings.json</c>) used for validation.
    /// </param>
    /// <exception cref="FileNotFoundException">
    /// Thrown if the schema file at the specified <paramref name="schemaPath" /> is not found.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the configuration is missing required keys, contains unexpected keys, or has invalid types for
    /// any of its values.
    /// </exception>
    /// <remarks>
    /// This method ensures that:
    /// <list type="bullet">
    /// <item>All required keys specified in the schema are present in the configuration.</item>
    /// <item>The type of each key in the configuration matches the type specified in the schema.</item>
    /// <item>No unexpected keys (i.e., keys not defined in the schema) are present in the configuration.</item>
    /// </list>
    /// </remarks>
    void ValidateExpandoWithSchema(ExpandoObject configuration, string schemaPath);
}