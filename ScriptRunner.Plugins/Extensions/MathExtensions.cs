using System;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for mathematical operations on numeric types.
/// </summary>
public static class MathExtensions
{
    /// <summary>
    ///     Adds two numbers and returns the result. Supports int and double.
    /// </summary>
    public static T Add<T>(this T a, T b) where T : IConvertible
    {
        return (T)Convert.ChangeType(Convert.ToDouble(a) + Convert.ToDouble(b), typeof(T));
    }

    /// <summary>
    ///     Multiplies two numbers and returns the result. Supports int and double.
    /// </summary>
    public static T Multiply<T>(this T a, T b) where T : IConvertible
    {
        return (T)Convert.ChangeType(Convert.ToDouble(a) * Convert.ToDouble(b), typeof(T));
    }

    /// <summary>
    ///     Subtracts the second number from the first and returns the result. Supports int and double.
    /// </summary>
    public static T Subtract<T>(this T a, T b) where T : IConvertible
    {
        return (T)Convert.ChangeType(Convert.ToDouble(a) - Convert.ToDouble(b), typeof(T));
    }

    /// <summary>
    ///     Divides two numbers and returns the result. Supports int and double. Throw exception on division by zero.
    /// </summary>
    public static T Divide<T>(this T a, T b) where T : IConvertible
    {
        if (Convert.ToDouble(b) == 0)
            throw new ArgumentException("Division by zero is not allowed.");
        return (T)Convert.ChangeType(Convert.ToDouble(a) / Convert.ToDouble(b), typeof(T));
    }

    /// <summary>
    ///     Raises the base number to the power of the exponent. Supports int and double.
    /// </summary>
    public static double Power<T>(this T baseNumber, T exponent) where T : IConvertible
    {
        return Math.Pow(Convert.ToDouble(baseNumber), Convert.ToDouble(exponent));
    }

    /// <summary>
    ///     Clamps a number to be within a specified range.
    /// </summary>
    /// <typeparam name="T">The type of the numeric values, which must be convertible.</typeparam>
    /// <param name="value">The number to clamp.</param>
    /// <param name="min">The minimum value of the range.</param>
    /// <param name="max">The maximum value of the range.</param>
    /// <returns>The clamped value within the specified range.</returns>
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        return value.CompareTo(max) > 0 ? max : value;
    }

    /// <summary>
    ///     Returns the absolute value of a number.
    /// </summary>
    /// <typeparam name="T">The type of the numeric value, which must be convertible.</typeparam>
    /// <param name="value">The number for which to find the absolute value.</param>
    /// <returns>The absolute value of the number.</returns>
    public static T Abs<T>(this T value) where T : IConvertible
    {
        return (T)Convert.ChangeType(Math.Abs(Convert.ToDouble(value)), typeof(T));
    }

    /// <summary>
    ///     Calculates the square root of a number.
    /// </summary>
    /// <typeparam name="T">The type of the numeric value, which must be convertible.</typeparam>
    /// <param name="value">The number for which to find the square root.</param>
    /// <returns>The square root of the number.</returns>
    /// <exception cref="ArgumentException">Thrown if the number is negative.</exception>
    public static double Sqrt<T>(this T value) where T : IConvertible
    {
        var doubleValue = Convert.ToDouble(value);
        if (doubleValue < 0)
            throw new ArgumentException("Square root is not defined for negative numbers.");
        return Math.Sqrt(doubleValue);
    }

    /// <summary>
    ///     Calculates the modulus (remainder) of dividing the first number by the second.
    /// </summary>
    /// <typeparam name="T">The type of the numeric values, which must be convertible.</typeparam>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The remainder after dividing <paramref name="a" /> by <paramref name="b" />.</returns>
    /// <exception cref="ArgumentException">Thrown if the divisor is zero.</exception>
    public static T Mod<T>(this T a, T b) where T : IConvertible
    {
        if (Convert.ToDouble(b) == 0)
            throw new ArgumentException("Division by zero is not allowed.");
        return (T)Convert.ChangeType(Convert.ToDouble(a) % Convert.ToDouble(b), typeof(T));
    }

    /// <summary>
    ///     Calculates the factorial of an integer.
    /// </summary>
    public static int Factorial(this int number)
    {
        if (number < 0)
            throw new ArgumentException("Factorial is not defined for negative numbers.");
        return number == 0 ? 1 : number * Factorial(number - 1);
    }
}