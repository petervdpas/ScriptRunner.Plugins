using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ScriptRunner.Plugins.Converters;

/// <summary>
/// A value converter that determines visibility based on an enumeration value.
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts an enumeration value to a visibility state by comparing it to a parameter.
    /// </summary>
    /// <param name="value">The enumeration value to compare. Expected to be of type <see cref="Enum"/>.</param>
    /// <param name="targetType">The type of the binding target property. This parameter is ignored.</param>
    /// <param name="parameter">The parameter to compare the enumeration value against. Expected to be of type <see cref="Enum"/> or a string representation of an enum value.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is ignored.</param>
    /// <returns>
    /// <c>true</c> if the <paramref name="value"/> matches the <paramref name="parameter"/>; otherwise, <c>false</c>.
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        return value.ToString()!.Equals(parameter.ToString());
    }

    /// <summary>
    /// Converts back from a visibility state to an enumeration value.
    /// </summary>
    /// <param name="value">The value to convert back. This parameter is ignored.</param>
    /// <param name="targetType">The type of the binding source property. This parameter is ignored.</param>
    /// <param name="parameter">An optional parameter. This parameter is ignored.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is ignored.</param>
    /// <returns>Throws a <see cref="NotImplementedException"/> as this operation is not supported.</returns>
    /// <exception cref="NotImplementedException">Always thrown as the method is not implemented.</exception>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}