using System;

namespace ScriptRunner.Plugins.Attributes;

/// <summary>
///     Provides metadata for plugins to describe their properties, behavior, and compatibility.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PluginMetadataAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginMetadataAttribute" /> class.
    /// </summary>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="description">A short description of the plugin.</param>
    /// <param name="author">The author of the plugin.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="pluginSystemVersion">The version of the plugin system this plugin was built for.</param>
    /// <param name="frameworkVersion">The target framework version (optional).</param>
    /// <param name="services">An array of service names provided by the plugin (optional).</param>
    public PluginMetadataAttribute(
        string name,
        string description,
        string author,
        string version,
        string pluginSystemVersion,
        string frameworkVersion = "",
        string[]? services = null)
    {
        Name = name;
        Description = description;
        Author = author;
        Version = version;
        PluginSystemVersion = pluginSystemVersion;
        FrameworkVersion = frameworkVersion;
        Services = services ?? [];
    }

    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets a short description of the plugin.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the author of the plugin.
    /// </summary>
    public string Author { get; }

    /// <summary>
    ///     Gets the version of the plugin.
    /// </summary>
    public string Version { get; }

    /// <summary>
    ///     Gets the version of the plugin system this plugin was built for.
    /// </summary>
    public string PluginSystemVersion { get; }

    /// <summary>
    ///     Gets the target framework version for the plugin (optional).
    /// </summary>
    public string FrameworkVersion { get; }

    /// <summary>
    ///     Gets the list of services provided by the plugin, if applicable.
    /// </summary>
    public string[]? Services { get; }
}