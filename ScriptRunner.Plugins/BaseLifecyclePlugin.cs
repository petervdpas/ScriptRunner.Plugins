using System.Collections.Generic;
using System.Dynamic;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for plugins that support lifecycle management.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of lifecycle-based plugins by providing default
///     behavior for lifecycle methods, while requiring derived classes to specify their unique name
///     and optionally override lifecycle methods as needed.
/// </remarks>
public abstract class BaseLifecyclePlugin : ILifecyclePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    public abstract string Name { get; }

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     initialization tasks, such as reading configuration values or setting up internal state.
    /// </remarks>
    public virtual void Initialize(ExpandoObject configuration)
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     the plugin's primary behavior.
    /// </remarks>
    public virtual void Execute()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin starts its operations.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     behavior that should occur when the plugin is started, such as initializing resources or starting timers.
    /// </remarks>
    public virtual void OnStart()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin stops its operations.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     behavior that should occur when the plugin is stopped, such as releasing locks or stopping timers.
    /// </remarks>
    public virtual void OnStop()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin is disposed and its resources are cleaned up.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     behavior that should occur when the plugin is disposed, such as releasing unmanaged resources or
    ///     unsubscribing from events.
    /// </remarks>
    public virtual void OnDispose()
    {
        // Default implementation: Do nothing
    }
}