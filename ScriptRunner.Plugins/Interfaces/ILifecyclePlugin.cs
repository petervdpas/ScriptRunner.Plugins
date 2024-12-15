namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a plugin that supports lifecycle management through distinct phases of operation.
/// </summary>
/// <remarks>
///     Plugins implementing this interface can manage resources or perform actions during specific
///     lifecycle phases, such as startup, shutdown, or disposal.
/// </remarks>
public interface ILifecyclePlugin : IPlugin
{
    /// <summary>
    ///     Called when the plugin starts its operations.
    /// </summary>
    /// <remarks>
    ///     This method is intended for tasks that should occur when the plugin is activated or
    ///     begins execution. Examples include initializing resources, starting timers, or setting up state.
    /// </remarks>
    void OnStart();

    /// <summary>
    ///     Called when the plugin stops its operations.
    /// </summary>
    /// <remarks>
    ///     This method is intended for tasks that should occur when the plugin is deactivated or
    ///     stops execution. Examples include stopping timers, releasing locks, or pausing operations.
    /// </remarks>
    void OnStop();

    /// <summary>
    ///     Called when the plugin is disposed and its resources are cleaned up.
    /// </summary>
    /// <remarks>
    ///     This method should perform cleanup tasks, such as releasing unmanaged resources,
    ///     unsubscribing from events, or closing open connections. It is the final stage of the plugin's lifecycle.
    /// </remarks>
    void OnDispose();
}