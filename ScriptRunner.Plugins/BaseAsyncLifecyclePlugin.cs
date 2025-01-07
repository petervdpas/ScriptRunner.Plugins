using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous lifecycle plugins.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous lifecycle plugins by providing
///     default asynchronous behavior for lifecycle methods.
/// </remarks>
public abstract class BaseAsyncLifecyclePlugin : IAsyncLifecyclePlugin, ILocalStorageConsumer
{
    private ILocalStorage? _localStorage;
    
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
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
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous initialization operation.</returns>
    public virtual async Task InitializeAsync(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Store settings into LocalStorage
        PluginSettingsHelper.StoreSettings(LocalStorage, configuration);
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously executes the plugin's main functionality.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous execution operation.</returns>
    public virtual async Task ExecuteAsync()
    {
        await Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously starts the plugin's operations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public virtual async Task OnStartAsync()
    {
        // Example: Retrieve settings on start
        var initialState = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "InitialState");
        Console.WriteLine($"{Name} started with initial state: {initialState}");
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously stops the plugin's operations.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public virtual async Task OnStopAsync()
    {
        Console.WriteLine($"{Name} is stopping...");
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously disposes of the plugin's resources.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous cleanup operation.</returns>
    public virtual async Task OnDisposeAsync()
    {
        Console.WriteLine($"{Name} is disposing...");
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
}