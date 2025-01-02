using ReactiveUI;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a field in a record's details, consisting of a label and a value.
/// </summary>
public class DetailField : ReactiveObject
{
    private string? _label;
    private string? _value;

    /// <summary>
    ///     Gets or sets the label of the detail field.
    /// </summary>
    /// <value>The label is a descriptive name for the field.</value>
    public string? Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }

    /// <summary>
    ///     Gets or sets the value of the detail field.
    /// </summary>
    /// <value>The value represents the data or content associated with the field.</value>
    public string? Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}