using System;

namespace ScriptRunner.Plugins.Logging;

/// <summary>
/// Defines a logging interface for plugins, enabling decoupled and customizable logging functionality.
/// </summary>
public interface IPluginLogger
{
    /// <summary>
    /// Logs a debug message, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The debug message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    void Debug(string message, params object[] args);

    /// <summary>
    /// Logs an informational message, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The informational message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    void Information(string message, params object[] args);

    /// <summary>
    /// Logs a warning message, optionally with structured arguments.
    /// </summary>
    /// <param name="message">The warning message to log. Can be null.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    void Warning(string message, params object[] args);

    /// <summary>
    /// Logs an error message, optionally with structured arguments or exception details.
    /// </summary>
    /// <param name="message">The error message to log. Can be null.</param>
    /// <param name="exception">An optional exception related to the error.</param>
    /// <param name="args">Optional arguments to format the message.</param>
    void Error(string message, Exception? exception = null, params object[] args);
}