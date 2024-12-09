using System.Reflection;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.Exceptions;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Default implementation of <see cref="IPluginValidator"/> for validating plugins.
/// </summary>
/// <remarks>
/// This class provides methods to validate plugin types to ensure they meet the required standards
/// for integration with the ScriptRunner framework.
/// </remarks>
public class PluginValidator : IPluginValidator
{
    /// <summary>
    /// Validates a plugin type to ensure it conforms to the required standards.
    /// </summary>
    /// <param name="pluginType">The type of the plugin to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginType"/> is null.</exception>
    /// <exception cref="PluginInitializationException">
    /// Thrown if the plugin lacks required metadata, does not implement <see cref="IPlugin"/>, or fails other validation checks.
    /// </exception>
    public void Validate(Type pluginType)
    {
        if (pluginType == null)
            throw new ArgumentNullException(nameof(pluginType), "Plugin type cannot be null.");

        var metadata = pluginType.GetCustomAttribute<PluginMetadataAttribute>();
        if (metadata == null)
            throw new PluginInitializationException($"Plugin {pluginType.Name} is missing the required PluginMetadataAttribute.");

        ValidateMetadata(metadata, pluginType);

        if (!typeof(IPlugin).IsAssignableFrom(pluginType))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must implement the IPlugin interface.");

        if (typeof(IServicePlugin).IsAssignableFrom(pluginType))
        {
            ValidateServicePlugin(pluginType, metadata);
        }
    }

    /// <summary>
    /// Validates the metadata attributes of a plugin to ensure required fields are provided.
    /// </summary>
    /// <param name="metadata">The metadata associated with the plugin.</param>
    /// <param name="pluginType">The type of the plugin being validated.</param>
    /// <exception cref="PluginInitializationException">
    /// Thrown if required metadata fields (e.g., Name, Version, or PluginSystemVersion) are missing or invalid.
    /// </exception>
    private static void ValidateMetadata(PluginMetadataAttribute metadata, Type pluginType)
    {
        var currentSystemVersion = PluginSystemInfo.CurrentPluginSystemVersion;

        if (string.IsNullOrWhiteSpace(metadata.Name))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must have a non-empty Name property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.Version))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must have a non-empty Version property in its metadata.");

        if (string.IsNullOrWhiteSpace(metadata.PluginSystemVersion))
            throw new PluginInitializationException($"Plugin {pluginType.Name} must specify the PluginSystemVersion.");

        if (metadata.PluginSystemVersion != currentSystemVersion)
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} is not compatible with the current plugin system version ({currentSystemVersion}). " +
                $"Required: {metadata.PluginSystemVersion}.");

        if (!string.IsNullOrWhiteSpace(metadata.FrameworkVersion) && !IsValidFrameworkVersion(metadata.FrameworkVersion))
            throw new PluginInitializationException($"Plugin {pluginType.Name} specifies an invalid framework version: {metadata.FrameworkVersion}");
    }

    /// <summary>
    /// Validates the declared services in a plugin's metadata to ensure they are implemented.
    /// </summary>
    /// <param name="pluginType">The plugin type being validated.</param>
    /// <param name="metadata">The metadata associated with the plugin.</param>
    /// <exception cref="PluginInitializationException">
    /// Thrown if declared services in the metadata are not implemented as methods in the plugin class.
    /// </exception>
    private static void ValidateServicePlugin(Type pluginType, PluginMetadataAttribute metadata)
    {
        if (metadata.Services == null || metadata.Services.Length == 0)
            return;

        var missingServices = metadata.Services.Where(service =>
            pluginType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .All(m => m.Name != service)).ToList();

        if (missingServices.Count != 0)
        {
            throw new PluginInitializationException(
                $"Plugin {pluginType.Name} declares the following services in its metadata but does not implement them: {string.Join(", ", missingServices)}.");
        }
    }

    /// <summary>
    /// Checks if the specified framework version string is valid.
    /// </summary>
    /// <param name="frameworkVersion">The framework version string to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the framework version string is valid; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// The framework version is considered valid if it starts with ".NET".
    /// </remarks>
    private static bool IsValidFrameworkVersion(string frameworkVersion)
    {
        return frameworkVersion.StartsWith(".NET");
    }
}