﻿using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for plugins that register services in the application's dependency injection
///     container.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of service-based plugins by providing default behavior for
///     initialization
///     and execution, while requiring derived classes to specify their name and service registrations.
/// </remarks>
public abstract class BaseServicePlugin : IServicePlugin, ILocalStorageConsumer
{
    /// <summary>
    ///     Sets the local storage instance for the plugin.
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
    /// <value>
    ///     A string representing the name of the plugin.
    /// </value>
    /// <remarks>
    ///     Derived classes must implement this property to provide a unique name for the plugin.
    /// </remarks>
    public abstract string Name { get; }

    /// <summary>
    ///     Registers the plugin's services in the provided dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the plugin's services should be added.</param>
    /// <remarks>
    ///     Derived classes must implement this method to define the services required by the plugin.
    /// </remarks>
    public abstract void RegisterServices(IServiceCollection services);

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">
    ///     A dictionary containing configuration key-value pairs for the plugin.
    /// </param>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     initialization tasks such as reading configuration values or setting up internal state.
    /// </remarks>
    public virtual void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
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
        // Default implementation: No-op
    }
}