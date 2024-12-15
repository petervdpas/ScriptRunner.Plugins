using System.Collections.Generic;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a plugin with initialization and execution capabilities.
/// </summary>
public interface IPlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    void Initialize(IDictionary<string, object> configuration);

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    void Execute();
}