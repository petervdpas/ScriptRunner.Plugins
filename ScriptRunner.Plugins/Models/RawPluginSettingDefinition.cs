namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents the raw definition of a plugin setting as loaded from a JSON configuration file.
/// </summary>
public class RawPluginSettingDefinition
{
    /// <summary>
    ///     Gets or sets the key of the setting.
    /// </summary>
    /// <remarks>
    ///     This corresponds to the unique identifier for the setting in the JSON file.
    /// </remarks>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the type of the setting.
    /// </summary>
    /// <remarks>
    ///     Defines the expected data type for the setting (e.g., "string", "int", "bool").
    ///     Defaults to "string" if not specified.
    /// </remarks>
    public string Type { get; set; } = "string";

    /// <summary>
    ///     Gets or sets the default value of the setting.
    /// </summary>
    /// <remarks>
    ///     This value is specified in the JSON file and represents the default value to use
    ///     when no user-defined value is provided.
    /// </remarks>
    public object? DefaultValue { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the setting is a secret.
    /// </summary>
    /// <remarks>
    ///     This field allows marking sensitive settings (e.g., API keys) to distinguish them
    ///     as secrets that require special handling.
    /// </remarks>
    public bool IsSecret { get; set; }
}