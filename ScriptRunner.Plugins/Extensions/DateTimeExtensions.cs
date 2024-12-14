using System;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for DateTime operations.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    ///     Formats the given date using the specified format string.
    /// </summary>
    /// <param name="date">The <see cref="DateTime" /> to format.</param>
    /// <param name="format">The format string to apply. Defaults to "yyyy-MM-dd".</param>
    /// <returns>A string representing the formatted date.</returns>
    public static string FormatDate(this DateTime date, string format = "yyyy-MM-dd")
    {
        return date.ToString(format);
    }

    /// <summary>
    ///     Returns the year component of the given <see cref="DateTime" />.
    /// </summary>
    /// <param name="date">The date from which to extract the year.</param>
    /// <returns>The year as an integer.</returns>
    public static int GetYear(this DateTime date)
    {
        return date.Year;
    }

    /// <summary>
    ///     Returns the month component of the given <see cref="DateTime" />.
    /// </summary>
    /// <param name="date">The date from which to extract the month.</param>
    /// <returns>The month as an integer.</returns>
    public static int GetMonth(this DateTime date)
    {
        return date.Month;
    }

    /// <summary>
    ///     Returns the day component of the given <see cref="DateTime" />.
    /// </summary>
    /// <param name="date">The date from which to extract the day.</param>
    /// <returns>The day as an integer.</returns>
    public static int GetDay(this DateTime date)
    {
        return date.Day;
    }

    /// <summary>
    ///     Calculates the difference between two <see cref="DateTime" /> objects and returns the result as a
    ///     <see cref="TimeSpan" />.
    /// </summary>
    /// <param name="startDate">The starting date and time.</param>
    /// <param name="endDate">The ending date and time.</param>
    /// <returns>A <see cref="TimeSpan" /> representing the difference between the two dates.</returns>
    public static TimeSpan GetTimeDifference(this DateTime startDate, DateTime endDate)
    {
        return endDate - startDate;
    }
}