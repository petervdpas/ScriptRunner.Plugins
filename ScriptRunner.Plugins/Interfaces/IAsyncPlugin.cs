using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents an asynchronous plugin that extends the functionality of <see cref="IPlugin" />.
/// </summary>
/// <remarks>
///     Plugins implementing this interface support asynchronous initialization and execution,
///     enabling non-blocking operations and better resource utilization for I/O-bound or long-running tasks.
/// </remarks>
public interface IAsyncPlugin : IPlugin
{
    /// <summary>
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing key-value pairs for plugin configuration.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous initialization operation.</returns>
    /// <remarks>
    ///     This method should be used to perform setup tasks that require asynchronous operations,
    ///     such as reading configurations from a remote source, establishing network connections, or preparing resources.
    /// </remarks>
    Task InitializeAsync(ExpandoObject configuration);

    /// <summary>
    ///     Asynchronously executes the main functionality of the plugin.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous execution operation.</returns>
    /// <remarks>
    ///     This method is intended to perform the core operations of the plugin, particularly
    ///     those that involve asynchronous or time-intensive tasks, such as API calls, file I/O, or data processing.
    /// </remarks>
    Task ExecuteAsync();
}