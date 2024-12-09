namespace ScriptRunner.Plugins.Attributes;

/// <summary>
/// Provides metadata for plugins to describe their properties and behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginMetadataAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a short description of the plugin.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the author of the plugin.
    /// </summary>
    public string Author { get; }
    
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Gets the target framework version for the plugin (optional).
    /// </summary>
    public string FrameworkVersion { get; }

    /// <summary>
    /// Gets the list of services provided by the plugin, if applicable.
    /// </summary>
    public string[]? Services { get; }
    
    /// <summary>
    /// Gets the list of plugins that this plugin depends on.
    /// </summary>
    /// <remarks>
    /// Dependencies are specified as plugin names that must be available for this plugin to function.
    /// </remarks>
    public string[]? Dependencies { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginMetadataAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="description">A short description of the plugin.</param>
    /// <param name="author">The author of the plugin.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="frameworkVersion">The target framework version (optional).</param>
    /// <param name="services">An array of service names provided by the plugin (optional).</param>
    /// <param name="dependencies">An array of plugin names that this plugin depends on (optional).</param>
    public PluginMetadataAttribute(
        string name,
        string description,
        string author,
        string version,
        string frameworkVersion = "",
        string[]? services = null,
        string[]? dependencies = null)
    {
        Name = name;
        Description = description;
        Author = author;
        Version = version;
        FrameworkVersion = frameworkVersion;
        Services = services ?? [];
        Dependencies = dependencies ?? [];
    }
}
