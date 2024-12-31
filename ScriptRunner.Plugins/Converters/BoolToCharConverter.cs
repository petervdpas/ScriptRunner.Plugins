using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ScriptRunner.Plugins.Converters;

/// <summary>
/// A value converter that maps a boolean value to a character.
/// </summary>
public class BoolToCharConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a character.
    /// </summary>
    /// <param name="value">The value to convert. Expected to be of type <see cref="bool"/>.</param>
    /// <param name="targetType">The type of the binding target property. This parameter is ignored.</param>
    /// <param name="parameter">An optional parameter. This parameter is ignored.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is ignored.</param>
    /// <returns>
    /// The character '*' if <paramref name="value"/> is <c>true</c>; otherwise, the null character ('\0').
    /// </returns>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? '*' : '\0';
    }

    /// <summary>
    /// Converts back from a character to a boolean value.
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