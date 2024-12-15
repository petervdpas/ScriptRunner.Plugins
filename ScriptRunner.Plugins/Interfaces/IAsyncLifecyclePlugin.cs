using System.Threading.Tasks;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents an asynchronous plugin that supports lifecycle management through distinct phases of operation.
/// </summary>
public interface IAsyncLifecyclePlugin : IAsyncPlugin
{
    /// <summary>
    ///     Asynchronously called when the plugin starts its operations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task OnStartAsync();

    /// <summary>
    ///     Asynchronously called when the plugin stops its operations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task OnStopAsync();

    /// <summary>
    ///     Asynchronously called when the plugin is disposed and its resources are cleaned up.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous cleanup operation.</returns>
    Task OnDisposeAsync();
}