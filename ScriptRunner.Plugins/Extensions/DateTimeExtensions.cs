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
    public static TimeSpan GetTimeDifference(this DateTime startDate, DateTime endDate)
    {
        return endDate - startDate;
    }

    /// <summary>
    ///     Converts the given <see cref="DateTime" /> to a Unix timestamp (seconds since January 1, 1970 UTC).
    /// </summary>
    public static long ToUnixTimestamp(this DateTime date)
    {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return Convert.ToInt64((date.ToUniversalTime() - unixEpoch).TotalSeconds);
    }

    /// <summary>
    ///     Converts a Unix timestamp (seconds since January 1, 1970 UTC) to a <see cref="DateTime" />.
    /// </summary>
    public static DateTime FromUnixTimestamp(this long unixTimestamp)
    {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return unixEpoch.AddSeconds(unixTimestamp);
    }

    /// <summary>
    ///     Returns the start of the day for the given <see cref="DateTime" />.
    /// </summary>
    public static DateTime StartOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, date.Kind);
    }

    /// <summary>
    ///     Returns the end of the day for the given <see cref="DateTime" />.
    /// </summary>
    public static DateTime EndOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);
    }

    /// <summary>
    ///     Adds the specified number of working days to the given <see cref="DateTime" />, skipping weekends.
    /// </summary>
    public static DateTime AddWorkingDays(this DateTime date, int workingDays)
    {
        if (workingDays == 0)
            return date;

        var direction = workingDays > 0 ? 1 : -1;
        var daysAdded = 0;

        while (daysAdded < Math.Abs(workingDays))
        {
            date = date.AddDays(direction);

            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                daysAdded++;
        }

        return date;
    }

    /// <summary>
    ///     Determines if the given <see cref="DateTime" /> is a weekend (Saturday or Sunday).
    /// </summary>
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    ///     Determines if the given <see cref="DateTime" /> is a weekday (Monday through Friday).
    /// </summary>
    public static bool IsWeekday(this DateTime date)
    {
        return !date.IsWeekend();
    }
    
    /// <summary>
    ///     Returns the number of days remaining in the month for the given <see cref="DateTime" />.
    /// </summary>
    public static int DaysRemainingInMonth(this DateTime date)
    {
        var lastDayOfMonth = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        return (lastDayOfMonth - date).Days;
    }

    /// <summary>
    ///     Calculates the total number of days in the month for the given <see cref="DateTime" />.
    /// </summary>
    public static int TotalDaysInMonth(this DateTime date)
    {
        return DateTime.DaysInMonth(date.Year, date.Month);
    }
}