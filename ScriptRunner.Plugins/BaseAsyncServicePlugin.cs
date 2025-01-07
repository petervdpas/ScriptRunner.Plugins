using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous service plugins.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous service plugins by providing
///     default asynchronous behavior for initialization and service registration.
/// </remarks>
public abstract class BaseAsyncServicePlugin : IAsyncServicePlugin, ILocalStorageConsumer
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
    /// <param name="localStorage">The local storage instance.</param>
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
    ///     Asynchronously registers the plugin's services into the provided DI container.
    /// </summary>
    public virtual async Task RegisterServicesAsync(IServiceCollection services)
    {
        // Default implementation: Store services in LocalStorage if required
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    public virtual async Task InitializeAsync(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(LocalStorage, configuration);
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously executes the plugin's main functionality.
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        // Default implementation: No-op
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Synchronously initializes the plugin. This is a wrapper for <see cref="InitializeAsync" />.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     Calls <see cref="InitializeAsync" /> to ensure compatibility with <see cref="IPlugin" />.
    ///     Includes unwrapping of any <see cref="AggregateException" /> to surface the inner exception.
    /// </remarks>
    public void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        try
        {
            InitializeAsync(configuration).Wait();
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    /// <summary>
    ///     Synchronously executes the plugin. This is a wrapper for <see cref="ExecuteAsync" />.
    /// </summary>
    /// <remarks>
    ///     Calls <see cref="ExecuteAsync" /> to ensure compatibility with <see cref="IPlugin" />.
    ///     Includes unwrapping of any <see cref="AggregateException" /> to surface the inner exception.
    /// </remarks>
    public void Execute()
    {
        try
        {
            ExecuteAsync().Wait();
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    /// <summary>
    ///     Synchronously registers the plugin's services. This is a wrapper for <see cref="RegisterServicesAsync" />.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <remarks>
    ///     Calls <see cref="RegisterServicesAsync" /> and unwraps any <see cref="AggregateException" /> to expose the
    ///     underlying exception.
    /// </remarks>
    public void RegisterServices(IServiceCollection services)
    {
        try
        {
            RegisterServicesAsync(services).Wait();
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }
}