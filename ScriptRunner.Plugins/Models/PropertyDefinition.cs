using System;
using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Defines a structure for property metadata in dynamically created classes.
/// </summary>
public class PropertyDefinition
{
    /// <summary>
    ///     Gets or sets the name of the property.
    /// </summary>
    /// <remarks>
    ///     This field specifies the name of the property in the dynamically created class.
    /// </remarks>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets or sets the string representation of the data type name.
    /// </summary>
    /// <remarks>
    ///     This property is used to serialize and deserialize the type information as JSON,
    ///     since <see cref="System.Type" /> instances are not directly serializable.
    /// </remarks>
    public required string TypeName { get; init; }

    /// <summary>
    ///     Gets the actual <see cref="System.Type" /> represented by <see cref="TypeName" />.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="System.String" /> if <see cref="TypeName" /> cannot be resolved.
    ///     This property is not settable and is derived directly from the <see cref="TypeName" />.
    /// </remarks>
    public Type Type => Type.GetType(TypeName) ?? typeof(string);

    /// <summary>
    ///     Gets or sets the type of UI control associated with this property.
    /// </summary>
    /// <remarks>
    ///     Examples include "TextBox", "ComboBox", and other UI control types.
    ///     This information determines how the property is rendered in a dynamic form or dialog.
    /// </remarks>
    public required string ControlType { get; init; }

    /// <summary>
    ///     Gets or sets the placeholder text to display in the UI control, if applicable.
    /// </summary>
    /// <remarks>
    ///     The placeholder provides a hint or example value for the user when interacting with the UI control.
    /// </remarks>
    public string Placeholder { get; init; } = "";

    /// <summary>
    ///     Gets or sets a collection of selectable options for controls like dropdowns or radio buttons.
    /// </summary>
    /// <remarks>
    ///     This property is typically used for controls that require pre-defined choices,
    ///     such as a DropDown UI or a set of Radio Buttons UI.
    /// </remarks>
    public IEnumerable<string>? Options { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether the property is required in the UI.
    /// </summary>
    /// <remarks>
    ///     If <c>true</c>, the UI will enforce that a value must be provided for this property.
    /// </remarks>
    public bool IsRequired { get; init; }

    /// <summary>
    ///     Gets or sets additional parameters to configure the control.
    /// </summary>
    /// <remarks>
    ///     Examples include "MaxLength" for a TextBox UI
    ///     or "Min" and "Max" for a Numeric-UpDown UI.
    ///     This allows flexible customization of the UI control's behavior.
    /// </remarks>
    public Dictionary<string, object>? ControlParameters { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether this property should be included as a display field.
    /// </summary>
    /// <remarks>
    ///     Display fields are typically used in summary or list views to represent the object to the user.
    /// </remarks>
    public bool IsDisplayField { get; init; }

    /// <summary>
    ///     Gets or sets configuration settings for dataset-level operations.
    /// </summary>
    /// <remarks>
    ///     This property is designed to hold configurations for grouping,
    ///     aggregations, and other dataset-level settings.
    ///     For example, it might define how this property is used in a chart or statistical analysis.
    /// </remarks>
    public Dictionary<string, object>? DataSetControls { get; init; }
}