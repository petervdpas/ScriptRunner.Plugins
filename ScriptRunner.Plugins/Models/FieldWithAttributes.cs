using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Specifies metadata for a property that will be displayed as a control in a dynamically generated form or dialog.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FieldWithAttributes : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FieldWithAttributes" /> class with the specified control type and
    ///     optional properties.
    /// </summary>
    /// <param name="controlType">The type of UI control to use for this field.</param>
    /// <param name="placeholder">Optional placeholder text to display within the control.</param>
    /// <param name="isRequired">Indicates if the field is required.</param>
    /// <param name="optionsJson">
    ///     A JSON-formatted string representing a collection of selectable options for the control.
    ///     This JSON string is deserialized into an <see cref="IEnumerable{T}" /> of strings, making it useful
    ///     for controls like ComboBox or RadioButton that offer multiple choices to the user.
    /// </param>
    /// <param name="controlParametersJson">
    ///     A JSON-formatted string containing additional configuration parameters specific to the control type.
    ///     These parameters are deserialized into a <see cref="Dictionary{TKey,TValue}" /> of string-object pairs,
    ///     allowing flexibility for control-specific settings such as maximum length, formatting, or validation rules.
    /// </param>
    /// <param name="isDisplayField">Indicates if the field is a display-field (for listing).</param>
    /// <param name="dataSetControlsJson">
    ///     A JSON-formatted string representing dataset-level configurations such as grouping or aggregation settings.
    ///     This JSON string is deserialized into a <see cref="Dictionary{TKey, TValue}" /> of string-object pairs, allowing
    ///     for advanced operations like data manipulation and analysis within the dataset.
    /// </param>
    public FieldWithAttributes(
        string controlType,
        string placeholder = "",
        bool isRequired = false,
        string optionsJson = "[]",
        string controlParametersJson = "{}",
        bool isDisplayField = false,
        string dataSetControlsJson = "{}")
    {
        ControlType = controlType;
        Placeholder = placeholder;
        IsRequired = isRequired;
        IsDisplayField = isDisplayField;

        // Deserialize the JSON strings back into the respective types
        Options = JsonSerializer.Deserialize<IEnumerable<string>>(optionsJson) ?? Array.Empty<string>();
        ControlParameters = JsonSerializer.Deserialize<Dictionary<string, object>>(controlParametersJson) ??
                            new Dictionary<string, object>();
        DataSetControls = JsonSerializer.Deserialize<Dictionary<string, object>>(dataSetControlsJson) ??
                          new Dictionary<string, object>();
    }

    /// <summary>
    ///     Gets the type of UI control to display for this field (e.g., "TextBox", "ComboBox").
    /// </summary>
    public string ControlType { get; }

    /// <summary>
    ///     Gets the placeholder text to display in the UI control, if applicable.
    /// </summary>
    public string Placeholder { get; }

    /// <summary>
    ///     Specifies if the field is required.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    ///     Provides options for controls like ComboBox or RadioButton.
    /// </summary>
    public IEnumerable<string> Options { get; }

    /// <summary>
    ///     Provides additional parameters specific to the control type.
    /// </summary>
    /// <remarks>
    ///     The parameters are deserialized from a JSON string into a dictionary of key-value pairs. These parameters
    ///     may include settings like "MaxLength" for a TextBox or "Min" and "Max" values for a NumericUpDown control.
    /// </remarks>
    public Dictionary<string, object> ControlParameters { get; }

    /// <summary>
    ///     Specifies if the field is part of the DisplayName (for listing purposes).
    /// </summary>
    /// <remarks>
    ///     Fields marked as display fields are typically included in summary or listing views of the dataset.
    /// </remarks>
    public bool IsDisplayField { get; }

    /// <summary>
    ///     Provides dataset-level configuration settings.
    /// </summary>
    /// <remarks>
    ///     This field is designed to hold configurations for grouping, aggregation, filtering, or other
    ///     dataset-wide operations. These settings are deserialized from a JSON string into a dictionary of key-value pairs.
    /// </remarks>
    public Dictionary<string, object> DataSetControls { get; }
}