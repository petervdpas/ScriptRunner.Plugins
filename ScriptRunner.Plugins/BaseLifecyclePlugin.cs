using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for plugins that support lifecycle management.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of lifecycle-based plugins by providing default
///     behavior for lifecycle methods, while requiring derived classes to specify their unique name
///     and optionally override lifecycle methods as needed.
/// </remarks>
public abstract class BaseLifecyclePlugin : ILifecyclePlugin, ILocalStorageConsumer
{
    private ILocalStorage? _localStorage;

    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    public abstract string Name { get; }

    /// <summary>
    ///     Sets the local storage instance for the plugin.
    /// </summary>
    /// <param name="localStorage">The local storage instance to associate with this plugin.</param>
    public void SetLocalStorage(ILocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    ///     Gets the local storage instance associated with the plugin.
    /// </summary>
    /// <returns>
    ///     The <see cref="ILocalStorage" /> instance associated with the plugin.
    /// </returns>
    public ILocalStorage GetLocalStorage()
    {
        return _localStorage ?? throw new InvalidOperationException("LocalStorage has not been set.");
    }
    
    /// <summary>
    ///     Gets the local storage instance for the plugin.
    /// </summary>
    protected ILocalStorage LocalStorage => _localStorage ?? throw new InvalidOperationException("LocalStorage is not set.");

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     initialization tasks, such as reading configuration values or setting up internal state.
    /// </remarks>
    public virtual void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(LocalStorage, configuration);
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
        // Example: Retrieve settings on start
        var initialState = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "InitialState");
        Console.WriteLine($"{Name} started with initial state: {initialState}");
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
        Console.WriteLine($"{Name} is stopping...");
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
        Console.WriteLine($"{Name} is disposing...");
    }
}