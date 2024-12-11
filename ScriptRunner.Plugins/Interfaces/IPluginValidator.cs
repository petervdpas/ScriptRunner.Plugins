using System;
using ScriptRunner.Plugins.Exceptions;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Represents a plugin validator that ensures plugins meet required standards for integration.
/// </summary>
public interface IPluginValidator
{
    /// <summary>
    /// Validates a plugin type to ensure it conforms to required standards.
    /// </summary>
    /// <param name="pluginType">The type of the plugin to validate.</param>
    /// <exception cref="PluginInitializationException">
    /// Thrown if the plugin lacks required metadata, is missing a name or version, or does not implement required interfaces.
    /// </exception>
    void Validate(Type pluginType);
}