using ReactiveUI;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a single configuration setting for a plugin.
/// </summary>
public class PluginSettingDefinition : ReactiveObject
{
    private string _key = string.Empty;
    private string _type = "string";
    private object? _value;

    /// <summary>
    ///     Gets or sets the key of the setting.
    /// </summary>
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    /// <summary>
    ///     Gets or sets the type of the setting (e.g., "string", "int", "bool").
    /// </summary>
    public string Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    /// <summary>
    ///     Gets or sets the value for the setting.
    /// </summary>
    public object? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}