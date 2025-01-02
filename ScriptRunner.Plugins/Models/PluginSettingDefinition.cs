namespace ScriptRunner.Plugins.Models;

/// <summary>
/// Represents a single configuration setting for a plugin.
/// </summary>
public class PluginSettingDefinition
{
    /// <summary>
    /// Gets or sets the key of the setting.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the setting (e.g., "string", "int", "bool").
    /// </summary>
    public string Type { get; set; } = "string";

    /// <summary>
    /// Gets or sets the default value for the setting.
    /// </summary>
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets a description of the setting.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}