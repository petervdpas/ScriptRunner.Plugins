using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Implementation of the IAvaloniaControlFactory interface for generating Avalonia controls.
/// </summary>
public class AvaloniaControlFactory : IAvaloniaControlFactory
{
    private readonly string[] _formats = ["yyyy-MM-dd", "dd/MM/yyyy"];
    private readonly ILogger<AvaloniaControlFactory> _logger;
    private ExpandoObject? _expando;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AvaloniaControlFactory" /> class.
    /// </summary>
    /// <param name="logger">
    ///     An instance of <see cref="ILogger{TCategoryName}" /> used for logging diagnostic and error information.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="logger" /> parameter is <c>null</c>.
    /// </exception>
    public AvaloniaControlFactory(ILogger<AvaloniaControlFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Generates Avalonia controls for each property in an ExpandoObject.
    /// </summary>
    /// <param name="expando">The ExpandoObject with dynamic properties to render as controls.</param>
    /// <returns>A collection of controls representing the properties of the ExpandoObject.</returns>
    public IEnumerable<Control> GenerateControls(ExpandoObject expando)
    {
        ArgumentNullException.ThrowIfNull(expando);

        var controls = new List<Control>();
        _expando = expando;

        IDictionary<string, object> expandoDict = _expando!;

        var keys = expandoDict.Keys.ToList();

        foreach (var key in keys)
            try
            {
                var value = expandoDict[key];

                // Create a label TextBlock for each property
                var label = new TextBlock
                {
                    Text = $"{key}:",
                    FontWeight = FontWeight.Bold,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                var control = CreateControlForValue(value, key);

                // Create a Grid to arrange the label and control
                var grid = new Grid();
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition(new GridLength(40, GridUnitType.Star))); // 40% for the label
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition(new GridLength(60, GridUnitType.Star))); // 60% for the control

                // Add the label and control to the Grid
                Grid.SetColumn(label, 0);
                Grid.SetColumn(control, 1);
                grid.Children.Add(label);
                grid.Children.Add(control);

                controls.Add(grid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating controls for ExpandoObject");
                throw;
            }

        return controls;
    }

    /// <summary>
    ///     Generates Avalonia controls for each property in a DataTable and binds them to DataRow values.
    /// </summary>
    /// <param name="dynamicType">The dynamic class type with properties to render as controls.</param>
    /// <param name="recordItem"></param>
    /// <returns>A collection of controls representing the properties of the dynamic class.</returns>
    public IEnumerable<Control> GenerateControls(Type? dynamicType, RecordItem recordItem)
    {
        var controls = new List<Control>();

        if (dynamicType == null) return controls;

        foreach (var property in dynamicType.GetProperties())
        {
            var attribute = property.GetCustomAttribute<FieldWithAttributes>();
            if (attribute == null) continue;

            try
            {
                // Create a label TextBlock with a fresh instance
                var label = new TextBlock
                {
                    Text = $"{property.Name}:",
                    FontWeight = FontWeight.Bold,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                Control control = attribute.ControlType switch
                {
                    "GeneratedIdTextBox" => CreateGeneratedIdTextBox(property, recordItem, attribute),
                    "TextBox" => ShouldUseMultiLine(attribute)
                        ? CreateMultiLineTextBox(property, recordItem, attribute)
                        : CreateFormattedTextBox(property, recordItem, attribute),
                    "ComboBox" => CreateComboBox(property, recordItem, attribute),
                    "NumericUpDown" => CreateNumericUpDown(property, recordItem, attribute),
                    "DatePicker" => CreateDatePicker(property, recordItem, attribute),
                    "RadioButton" => CreateRadioButtonGroup(property, recordItem, attribute),
                    "CheckBox" => CreateCheckBox(property, recordItem, attribute),
                    _ => new TextBlock { Text = $"Unsupported control type: {attribute.ControlType}" }
                };

                control.Margin = new Thickness(0, 5, 0, 2);

                var container = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Children = { label, control }
                };

                controls.Add(container);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating controls for type {DynamicType}", dynamicType);
                throw;
            }
        }

        return controls;
    }

    /// <summary>
    ///     Creates a <see cref="TextBox" /> control configured as a non-editable field for generated identifiers.
    /// </summary>
    /// <param name="property">
    ///     The <see cref="PropertyInfo" /> object representing the property associated with this control.
    /// </param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">
    ///     The <see cref="FieldWithAttributes" /> instance containing metadata for configuring the control.
    /// </param>
    /// <returns>
    ///     A <see cref="TextBox" /> control set to be read-only and initialized with a default, non-empty value.
    /// </returns>
    /// <remarks>
    ///     The created <see cref="TextBox" /> is configured to display a non-editable identifier value,
    ///     defaulting to "0" if the data row's value is empty or null. The control also enforces the
    ///     field's non-empty constraint by subscribing to the <see cref="TextBox.TextProperty" /> observable and
    ///     reassigning the default value if the user clears the field.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="property" /> or <paramref /> is null.
    /// </exception>
    private static TextBox CreateGeneratedIdTextBox(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        var dataRow = recordItem.DataRow;

        var textBox = new TextBox
        {
            Text = dataRow[property.Name].ToString() ?? "0",
            IsReadOnly = true,
            IsEnabled = false
        };

        // Ensure the value is non-empty and enforced
        textBox.GetObservable(TextBox.TextProperty).Subscribe(text =>
        {
            if (string.IsNullOrEmpty(text))
                textBox.Text = dataRow[property.Name].ToString() ?? "0";
            else
                dataRow[property.Name] = text;
        });

        return textBox;
    }

    /// <summary>
    ///     Creates a multi-line TextBox with word wrapping, bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the TextBox.</param>
    /// <returns>A configured TextBox control.</returns>
    private static TextBox CreateMultiLineTextBox(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        var textBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            MinHeight = 100,
            Text = recordItem.DataRow[property.Name].ToString() ?? string.Empty,
            Watermark = attribute.Placeholder
        };

        textBox.GetObservable(TextBox.TextProperty).Subscribe(text =>
        {
            recordItem.DataRow[property.Name] = text;
            recordItem.MarkAsDirty();
        });

        return textBox;
    }

    /// <summary>
    ///     Creates a formatted TextBox, applying constraints such as integer-only or uppercase formatting, and binds it to a
    ///     DataRow.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the TextBox.</param>
    /// <returns>A configured TextBox with formatting constraints.</returns>
    private static TextBox CreateFormattedTextBox(PropertyInfo property, RecordItem recordItem,
        FieldWithAttributes attribute)
    {
        var textBox = new TextBox
        {
            Watermark = attribute.Placeholder,
            Text = recordItem.DataRow[property.Name].ToString() ?? string.Empty
        };

        // Subscribe to TextProperty for handling various types of input constraints
        textBox.GetObservable(TextBox.TextProperty).Subscribe(text =>
        {
            if (string.IsNullOrEmpty(text))
            {
                recordItem.DataRow[property.Name] = string.Empty;
                return;
            }

            var filteredText = text;

            // Apply various transformations or constraints based on attribute parameters
            if (attribute.ControlParameters.TryGetValue("InputType", out var inputType))
                switch (inputType)
                {
                    case "Integer":
                        filteredText = new string(text.Where(char.IsDigit).ToArray());
                        break;
                    case "Decimal":
                        filteredText = FilterDecimal(text);
                        break;
                    case "Uppercase":
                        filteredText = text.ToUpper();
                        break;
                    case "Regex":
                        if (attribute.ControlParameters.TryGetValue("Pattern", out var pattern) &&
                            pattern is string regexPattern)
                            filteredText = ApplyRegexFilter(text, regexPattern);

                        break;
                }

            // If filtering modified the text, update the TextBox and caret position
            if (filteredText != text)
            {
                textBox.Text = filteredText;
                textBox.CaretIndex = filteredText.Length; // Keep caret at the end
            }

            recordItem.DataRow[property.Name] = filteredText;
            recordItem.MarkAsDirty();
        });

        return textBox;
    }

    /// <summary>
    ///     Creates a ComboBox bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the ComboBox.</param>
    /// <returns>A configured ComboBox control.</returns>
    private static ComboBox CreateComboBox(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        var comboBox = new ComboBox
        {
            ItemsSource = attribute.Options,
            SelectedItem = recordItem.DataRow[property.Name].ToString()
        };

        comboBox.GetObservable(SelectingItemsControl.SelectedItemProperty).Subscribe(selectedItem =>
        {
            recordItem.DataRow[property.Name] = selectedItem?.ToString();
            recordItem.MarkAsDirty();
        });

        return comboBox;
    }

    /// <summary>
    ///     Creates a NumericUpDown control bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the NumericUpDown control.</param>
    /// <returns>A configured NumericUpDown control.</returns>
    private static NumericUpDown CreateNumericUpDown(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        decimal min = 0;
        decimal max = 100;

        // Safely retrieve and convert Min and Max values
        if (attribute.ControlParameters.TryGetValue(
                "Min", out var minElement) &&
            minElement is JsonElement minJson &&
            minJson.TryGetDecimal(out var minValue))
            min = minValue;

        if (attribute.ControlParameters.TryGetValue(
                "Max", out var maxElement) &&
            maxElement is JsonElement maxJson &&
            maxJson.TryGetDecimal(out var maxValue))
            max = maxValue;

        var numericControl = new NumericUpDown
        {
            Minimum = min,
            Maximum = max,
            Value = Convert.ToDecimal(recordItem.DataRow[property.Name])
        };

        numericControl.GetObservable(NumericUpDown.ValueProperty)
            .Subscribe(value =>
            {
                recordItem.DataRow[property.Name] = value;
                recordItem.MarkAsDirty();
            });

        return numericControl;
    }

    /// <summary>
    ///     Creates a DatePicker control bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the DatePicker.</param>
    /// <returns>A configured DatePicker control.</returns>
    private DatePicker CreateDatePicker(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        DateTime? dateValue = null;

        switch (recordItem.DataRow[property.Name])
        {
            // Attempt to parse the date using known formats
            case string dateString:
                if (DateTime.TryParseExact(dateString, _formats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var parsedDate))
                    // Ensure date is within the allowable range for DateTimeOffset
                    dateValue = IsDateInRange(parsedDate) ? parsedDate : null;
                break;
            case DateTime dbDate:
                dateValue = IsDateInRange(dbDate) ? dbDate : null;
                break;
        }

        var datePicker = new DatePicker
        {
            SelectedDate = dateValue
        };

        datePicker.GetObservable(DatePicker.SelectedDateProperty).Subscribe(selectedDate =>
        {
            // Save back in 'yyyy-MM-dd' format for SQLite, if within range
            recordItem.DataRow[property.Name] = selectedDate.HasValue && IsDateInRange(selectedDate.Value.DateTime)
                ? selectedDate.Value.DateTime.ToString("yyyy-MM-dd")
                : DBNull.Value;
            recordItem.MarkAsDirty();
        });

        return datePicker;
    }

    /// <summary>
    ///     Creates a group of RadioButton controls bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the RadioButton group.</param>
    /// <returns>A StackPanel containing the configured RadioButton controls.</returns>
    private static StackPanel CreateRadioButtonGroup(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

        foreach (var option in attribute.Options)
        {
            var radioButton = new RadioButton
            {
                Content = option,
                IsChecked = option == recordItem.DataRow[property.Name].ToString()
            };

            radioButton.GetObservable(ToggleButton.IsCheckedProperty).Subscribe(_ =>
            {
                recordItem.DataRow[property.Name] = option;
                recordItem.MarkAsDirty();
            });

            stackPanel.Children.Add(radioButton);
        }

        return stackPanel;
    }

    /// <summary>
    ///     Creates a CheckBox control bound to a DataRow value.
    /// </summary>
    /// <param name="property">The property associated with this control.</param>
    /// <param name="recordItem"></param>
    /// <param name="attribute">Metadata attributes for configuring the CheckBox.</param>
    /// <returns>A configured CheckBox control.</returns>
    private static CheckBox CreateCheckBox(
        PropertyInfo property, RecordItem recordItem, FieldWithAttributes attribute)
    {
        var useNumericBoolean = attribute.ControlParameters.ContainsKey("UseNumericBoolean") &&
                                attribute.ControlParameters["UseNumericBoolean"] is JsonElement
                                {
                                    ValueKind: JsonValueKind.True
                                };

        var currentValue = recordItem.DataRow[property.Name];

        // Initialize CheckBox based on the type of value in DataRow and UseNumericBoolean flag
        var initialValue = useNumericBoolean
            ? Convert.ToInt32(currentValue) == 1
            : currentValue as bool? ?? false;

        var checkBox = new CheckBox
        {
            IsChecked = initialValue
        };

        checkBox.GetObservable(ToggleButton.IsCheckedProperty).Subscribe(isChecked =>
        {
            if (useNumericBoolean)
                // Set as 1 or 0 for numeric boolean
                recordItem.DataRow[property.Name] = isChecked == true ? 1 : 0;
            else
                // Set as true/false for regular boolean
                recordItem.DataRow[property.Name] = isChecked ?? false;

            recordItem.MarkAsDirty();
        });

        return checkBox;
    }

    /// <summary>
    ///     Filters an input string to retain only numeric digits and, at most, one decimal point.
    /// </summary>
    /// <param name="input">The input string to filter for numeric and decimal characters.</param>
    /// <returns>A string containing only numeric digits and a single decimal point if present in the original input.</returns>
    private static string FilterDecimal(string input)
    {
        var decimalPointSeen = false;
        return new string(input.Where(c =>
        {
            if (char.IsDigit(c)) return true;
            if (c != '.' || decimalPointSeen) return false;
            decimalPointSeen = true;
            return true;
        }).ToArray());
    }

    /// <summary>
    ///     Applies a regular expression pattern to filter the input string and retrieve matching content.
    /// </summary>
    /// <param name="input">The input string to filter.</param>
    /// <param name="pattern">The regular expression pattern used to match content in the input string.</param>
    /// <returns>The content in the input string that matches the specified pattern, or an empty string if no match is found.</returns>
    private static string ApplyRegexFilter(string input, string pattern)
    {
        var regex = new Regex(pattern);
        var match = regex.Match(input);
        return match.Success ? match.Value : string.Empty;
    }

    /// <summary>
    ///     Determines if a multiline text box should be used based on attribute parameters.
    /// </summary>
    /// <param name="attribute">The <see cref="FieldWithAttributes" /> instance containing control parameters.</param>
    /// <returns><c>true</c> if the multiline option (AcceptsReturn) is enabled; otherwise, <c>false</c>.</returns>
    private static bool ShouldUseMultiLine(FieldWithAttributes attribute)
    {
        return attribute.ControlParameters.TryGetValue("AcceptsReturn", out var acceptsReturn) && acceptsReturn is true;
    }

    /// <summary>
    ///     Determines if the specified <see cref="DateTime" /> falls within the allowable range
    ///     for <see cref="DateTimeOffset" />.
    /// </summary>
    /// <param name="date">
    ///     The <see cref="DateTime" /> value to validate against the minimum and maximum allowable dates.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the specified date is within the valid range for <see cref="DateTimeOffset" />;
    ///     otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     This method checks if the provided <see cref="DateTime" /> falls between
    ///     <see cref="DateTime.MinValue" /> and <see cref="DateTime.MaxValue" />,
    ///     which represent the allowable date range in .NET.
    /// </remarks>
    private static bool IsDateInRange(DateTime date)
    {
        return date >= DateTime.MinValue && date <= DateTime.MaxValue;
    }

    /// <summary>
    ///     Creates an Avalonia control based on the value type and binds it to the specified key in the associated
    ///     ExpandoObject.
    /// </summary>
    /// <param name="value">
    ///     The value associated with the key. The type of the value determines the type of control created.
    ///     Supported types include <see cref="string" />, <see cref="int" />, <see cref="bool" />, and <see cref="DateTime" />
    ///     .
    /// </param>
    /// <param name="key">
    ///     The key in the <see cref="ExpandoObject" /> that the control's binding will reference.
    /// </param>
    /// <returns>
    ///     A <see cref="Control" /> instance that is dynamically created and bound to the specified key and value.
    /// </returns>
    /// <remarks>
    ///     The method dynamically creates an appropriate control based on the type of the <paramref name="value" />:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><see cref="TextBox" /> for <see cref="string" /> values.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="NumericUpDown" /> for <see cref="int" /> values.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="CheckBox" /> for <see cref="bool" /> values.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="DatePicker" /> for <see cref="DateTime" /> values.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="TextBox" /> for unsupported types.</description>
    ///         </item>
    ///     </list>
    ///     Each control is bound to the corresponding key in the <see cref="ExpandoObject" />, allowing two-way
    ///     synchronization
    ///     between the control and the underlying data source.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="value" /> or <paramref name="key" /> is null.
    /// </exception>
    private Control CreateControlForValue(object? value, string key)
    {
        Control control;

        switch (value)
        {
            case string stringValue:
                var textBox = new TextBox
                {
                    Text = stringValue,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                // Bind the Text property to the ExpandoObject value
                textBox.Bind(TextBox.TextProperty, new Binding
                {
                    Path = key,
                    Source = _expando,
                    Mode = BindingMode.TwoWay
                });

                // Observe changes to the Text property and update _expando
                textBox.GetObservable(TextBox.TextProperty).Subscribe(newValue =>
                {
                    if (_expando != null) ((IDictionary<string, object?>)_expando)[key] = newValue;
                });

                control = textBox;
                break;

            case int intValue:
                var numericUpDown = new NumericUpDown
                {
                    Value = intValue,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                // Bind the Value property to the ExpandoObject value
                numericUpDown.Bind(NumericUpDown.ValueProperty, new Binding
                {
                    Path = key,
                    Source = _expando,
                    Mode = BindingMode.TwoWay
                });

                // Observe changes to the Value property and update _expando
                numericUpDown.GetObservable(NumericUpDown.ValueProperty).Subscribe(newValue =>
                {
                    if (_expando != null) ((IDictionary<string, object?>)_expando)[key] = newValue;
                });

                control = numericUpDown;
                break;

            case bool boolValue:
                var checkBox = new CheckBox
                {
                    IsChecked = boolValue,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                // Bind the IsChecked property to the ExpandoObject value
                checkBox.Bind(ToggleButton.IsCheckedProperty, new Binding
                {
                    Path = key,
                    Source = _expando,
                    Mode = BindingMode.TwoWay
                });

                // Observe changes to the IsChecked property and update _expando
                checkBox.GetObservable(ToggleButton.IsCheckedProperty).Subscribe(newValue =>
                {
                    if (_expando != null) ((IDictionary<string, object?>)_expando)[key] = newValue ?? false;
                });

                control = checkBox;
                break;

            case DateTime dateTimeValue:
                var datePicker = new DatePicker
                {
                    SelectedDate = dateTimeValue,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                // Bind the SelectedDate property to the ExpandoObject value
                datePicker.Bind(DatePicker.SelectedDateProperty, new Binding
                {
                    Path = key,
                    Source = _expando,
                    Mode = BindingMode.TwoWay
                });

                // Observe changes to the SelectedDate property and update _expando
                datePicker.GetObservable(DatePicker.SelectedDateProperty).Subscribe(newValue =>
                {
                    if (_expando != null)
                        ((IDictionary<string, object?>)_expando)[key] = newValue?.DateTime ?? DateTime.MinValue;
                });

                control = datePicker;
                break;

            default:
                control = new TextBox
                {
                    Text = value?.ToString() ?? string.Empty,
                    Margin = new Thickness(0, 5, 0, 2)
                };

                // Bind the Text property to the ExpandoObject value for unsupported types
                control.Bind(TextBox.TextProperty, new Binding
                {
                    Path = key,
                    Source = _expando,
                    Mode = BindingMode.TwoWay
                });

                // Observe changes to the Text property and update _expando
                control.GetObservable(TextBox.TextProperty).Subscribe(newValue =>
                {
                    if (_expando != null) ((IDictionary<string, object?>)_expando)[key] = newValue;
                });

                break;
        }

        return control;
    }
}