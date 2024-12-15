namespace ScriptRunner.Plugins.Logging;

/// <summary>
///     Defines the severity levels for log messages.
/// </summary>
/// <remarks>
///     Log levels categorize the importance and type of log messages, enabling developers
///     to filter and prioritize logs based on severity.
/// </remarks>
public enum LogLevel
{
    /// <summary>
    ///     Debug level for detailed and diagnostic information used during development.
    /// </summary>
    Debug,

    /// <summary>
    ///     Informational level for general operational messages that highlight the flow of the application.
    /// </summary>
    Info,

    /// <summary>
    ///     Warning level for potentially harmful situations that are not immediately critical.
    /// </summary>
    Warning,

    /// <summary>
    ///     Error level for serious issues that have caused a failure in a part of the application.
    /// </summary>
    Error,

    /// <summary>
    ///     Critical level for severe issues that may lead to the application shutting down or data loss.
    /// </summary>
    Critical
}