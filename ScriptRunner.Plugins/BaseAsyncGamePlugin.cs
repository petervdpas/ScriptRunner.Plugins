using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous game plugins.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous game plugins by providing
///     default no-op implementations for lifecycle and game loop methods.
/// </remarks>
public abstract class BaseAsyncGamePlugin : IAsyncGamePlugin, ILocalStorageConsumer
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
    ///     Asynchronously starts the plugin's operations.
    /// </summary>
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
    public virtual async Task OnStopAsync()
    {
        Console.WriteLine($"{Name} is stopping...");
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously disposes of the plugin's resources.
    /// </summary>
    public virtual async Task OnDisposeAsync()
    {
        Console.WriteLine($"{Name} is disposing...");
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously updates the game logic during each frame.
    /// </summary>
    public virtual async Task UpdateAsync()
    {
        Console.WriteLine($"{Name} is updating...");
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously renders the game visuals during each frame.
    /// </summary>
    public virtual async Task RenderAsync()
    {
        Console.WriteLine($"{Name} is rendering...");
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