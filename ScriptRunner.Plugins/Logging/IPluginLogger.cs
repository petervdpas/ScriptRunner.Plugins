namespace ScriptRunner.Plugins.Logging;

/// <summary>
/// Defines a logging interface for plugins, enabling decoupled and customizable logging functionality.
/// </summary>
public interface IPluginLogger
{
    /// <summary>
    /// Logs a debug message, typically used for diagnostic purposes.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    void Debug(string message);

    /// <summary>
    /// Logs an informational message, typically used for general-purpose information about application flow.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    void Information(string message);

    /// <summary>
    /// Logs a warning message, typically used to highlight potential issues or non-critical problems.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    void Warning(string message);

    /// <summary>
    /// Logs an error message, typically used to record exceptions or critical issues.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">An optional exception related to the error.</param>
    void Error(string message, Exception? exception = null);
}