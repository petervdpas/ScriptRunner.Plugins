using System;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Logging;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
/// Provides a wrapper to adapt <see cref="Microsoft.Extensions.Logging.ILogger"/> 
/// to the <see cref="ScriptRunner.Plugins.Logging.IPluginLogger"/> interface.
/// </summary>
public class PluginLogger : IPluginLogger
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLogger"/> class.
    /// </summary>
    /// <param name="logger">
    /// The underlying <see cref="ILogger"/> instance used for logging.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="logger"/> is null.
    /// </exception>
    public PluginLogger(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    public void Debug(string message)
    {
        _logger.LogDebug(message);
    }

    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The informational message to log.</param>
    public void Information(string message)
    {
        _logger.LogInformation(message);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void Warning(string message)
    {
        _logger.LogWarning(message);
    }

    /// <summary>
    /// Logs an error message, optionally including exception details.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="exception">
    /// The exception to include in the log. If null, only the message is logged.
    /// </param>
    public void Error(string message, Exception? exception = null)
    {
        if (exception != null)
        {
            _logger.LogError(exception, message);
        }
        else
        {
            _logger.LogError(message);
        }
    }
}