using System;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// A basic implementation of <see cref="IPluginLogger"/> that logs messages to the console.
/// </summary>
public class ConsolePluginLogger : IPluginLogger
{
    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    public void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");

    /// <summary>
    /// Logs an informational message to the console.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    public void Information(string message) => Console.WriteLine($"[INFO] {message}");

    /// <summary>
    /// Logs a warning message to the console.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void Warning(string message) => Console.WriteLine($"[WARNING] {message}");

    /// <summary>
    /// Logs an error message to the console, with an optional exception.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">An optional exception to include in the log output.</param>
    public void Error(string message, Exception? exception = null)
    {
        Console.WriteLine($"[ERROR] {message}");
        if (exception != null) Console.WriteLine($"[ERROR] Exception: {exception}");
    }
}