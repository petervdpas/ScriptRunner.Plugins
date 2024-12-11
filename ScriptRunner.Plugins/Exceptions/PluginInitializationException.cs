using System;

namespace ScriptRunner.Plugins.Exceptions;

/// <summary>
/// Represents an exception that occurs during the initialization of a plugin.
/// </summary>
public class PluginInitializationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginInitializationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PluginInitializationException(string message) : base(message) { }
}