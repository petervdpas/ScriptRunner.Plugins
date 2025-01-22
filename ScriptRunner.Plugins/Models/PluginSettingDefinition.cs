using System.Reactive;
using ReactiveUI;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a single configuration setting for a plugin, including options for masking-sensitive data.
/// </summary>
public class PluginSettingDefinition : ReactiveObject
{
    private string _key = string.Empty;
    private string _type = "string";
    private object? _value;
    private bool _isSecret;
    private bool _isMasked;
    private bool _showToggleOption;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginSettingDefinition"/> class.
    ///     Sets up the <see cref="ToggleMaskCommand"/> for toggling the masked state of the value.
    /// </summary>
    public PluginSettingDefinition()
    {
        ToggleMaskCommand = ReactiveCommand.Create(() =>
        {
            IsMasked = !IsMasked; // Toggle mask state
        });
    }
    
    /// <summary>
    ///     Gets or sets the key of the setting.
    ///     This is the unique identifier for the setting.
    /// </summary>
    public string Key
    {
        get => _key;
        set => this.RaiseAndSetIfChanged(ref _key, value);
    }

    /// <summary>
    ///     Gets or sets the type of the setting.
    ///     Example types include "string", "int", and "bool".
    /// </summary>
    public string Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    /// <summary>
    ///     Gets or sets the value of the setting.
    /// </summary>
    /// <remarks>
    ///     The value can be of any type, but it must match the type specified by <see cref="Type"/>.
    /// </remarks>
    public object? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
    
    /// <summary>
    ///     Gets or sets a value indicating whether the setting is a secret.
    ///     Secrets are values such as passwords or tokens that should not be displayed in plain text.
    /// </summary>
    public bool IsSecret
    {
        get => _isSecret;
        set
        {
            this.RaiseAndSetIfChanged(ref _isSecret, value);

            // Automatically mask the value if it's secret
            if (_isSecret)
            {
                IsMasked = true;
            }
        }
    }
    
    /// <summary>
    ///     Gets or sets a value indicating whether the value is currently masked.
    ///     If <c>true</c>, the value is displayed in a masked format, such as asterisks.
    /// </summary>
    public bool IsMasked
    {
        get => _isMasked;
        set => this.RaiseAndSetIfChanged(ref _isMasked, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the toggle option for masking is displayed.
    ///     This is typically used to determine if a "Show/Hide" button should be visible in the UI.
    /// </summary>
    public bool ShowToggleOption
    {
        get => _showToggleOption;
        set => this.RaiseAndSetIfChanged(ref _showToggleOption, value);
    }
    
    /// <summary>
    ///     Gets a command that toggles the <see cref="IsMasked"/> state.
    ///     This command can be bound to a UI element, such as a button, to allow users to show or hide the value.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ToggleMaskCommand { get; }
}