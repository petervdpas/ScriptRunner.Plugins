using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ScriptRunner.Plugins.Converters;

/// <summary>
///     A value converter that inverts a boolean value.
/// </summary>
public class BoolToInverseConverter : IValueConverter
{
    /// <summary>
    ///     Converts a boolean value to its inverse.
    /// </summary>
    /// <param name="value">The value to convert. Expected to be of type <see cref="bool" />.</param>
    /// <param name="targetType">The type of the binding target property. This parameter is ignored.</param>
    /// <param name="parameter">An optional parameter. This parameter is ignored.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is ignored.</param>
    /// <returns>The inverted boolean value, or <c>false</c> if the input is not a boolean.</returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue) return !boolValue;
        return false;
    }

    /// <summary>
    ///     Converts back an inverted boolean value to its original value.
    /// </summary>
    /// <param name="value">The value to convert back. Expected to be of type <see cref="bool" />.</param>
    /// <param name="targetType">The type of the binding source property. This parameter is ignored.</param>
    /// <param name="parameter">An optional parameter. This parameter is ignored.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is ignored.</param>
    /// <returns>The inverted boolean value, or <c>false</c> if the input is not a boolean.</returns>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue) return !boolValue;
        return false;
    }
}