using System;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     A basic implementation of <see cref="IPluginLogger" /> that logs messages to the console.
/// </summary>
public class ConsolePluginLogger : IPluginLogger
{
    /// <summary>
    ///     Logs a debug message to the console, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The debug message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    public void Debug(string? message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message)) Console.WriteLine($"[DEBUG] {FormatMessage(message, args)}");
    }

    /// <summary>
    ///     Logs an informational message to the console, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The informational message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    public void Information(string? message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message)) Console.WriteLine($"[INFO] {FormatMessage(message, args)}");
    }

    /// <summary>
    ///     Logs a warning message to the console, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The warning message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    public void Warning(string? message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message)) Console.WriteLine($"[WARNING] {FormatMessage(message, args)}");
    }

    /// <summary>
    ///     Logs an error message to the console, optionally with structured arguments or an exception.
    /// </summary>
    /// <param name="message">The error message to log. Can be null.</param>
    /// <param name="exception">An optional exception to include in the log output.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    public void Error(string? message, Exception? exception = null, params object[] args)
    {
        if (!string.IsNullOrEmpty(message)) Console.WriteLine($"[ERROR] {FormatMessage(message, args)}");

        if (exception != null) Console.WriteLine($"[ERROR] Exception: {exception}");
    }

    /// <summary>
    ///     Formats a message template with the provided arguments.
    /// </summary>
    /// <param name="message">The message template to format.</param>
    /// <param name="args">The arguments to inject into the template.</param>
    /// <returns>The formatted message.</returns>
    private static string FormatMessage(string message, object[] args)
    {
        try
        {
            return string.Format(message, args);
        }
        catch
        {
            // Fallback to the raw message if formatting fails
            return message;
        }
    }
}