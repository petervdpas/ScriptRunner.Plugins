﻿using System.Collections.Generic;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for plugins with initialization and execution capabilities.
/// </summary>
/// <remarks>
///     This abstract class simplifies plugin creation by providing default behavior for initialization and execution,
///     while requiring derived classes to define a unique name.
/// </remarks>
public abstract class BasePlugin : IPlugin, ILocalStorageConsumer
{
    /// <summary>
    ///     Allows the plugin host to provide an ILocalStorage instance.
    /// </summary>
    /// <param name="localStorage">The local storage instance to associate with this plugin.</param>
    public void SetLocalStorage(ILocalStorage? localStorage)
    {
        PluginSettingsHelper.InitializeLocalStorage(localStorage);
    }

    /// <summary>
    ///     Gets the local storage instance associated with the plugin.
    /// </summary>
    /// <returns>
    ///     The <see cref="ILocalStorage" /> instance associated with the plugin.
    /// </returns>
    public ILocalStorage GetLocalStorage()
    {
        return PluginSettingsHelper.FetchLocalStorage();
    }

    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    /// <remarks>
    ///     Derived classes must implement this property to provide a unique identifier for the plugin.
    /// </remarks>
    public abstract string Name { get; }

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     initialization tasks such as reading configuration values or setting up internal state.
    /// </remarks>
    public virtual void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Default implementation: Store settings in LocalStorage
        PluginSettingsHelper.StoreSettings(configuration);
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
}