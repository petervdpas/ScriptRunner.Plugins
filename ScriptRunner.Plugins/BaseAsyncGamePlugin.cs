using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous game plugins.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous game plugins by providing
///     default no-op implementations for lifecycle and game loop methods.
/// </remarks>
public abstract class BaseAsyncGamePlugin : IAsyncGamePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    public virtual Task InitializeAsync(ExpandoObject configuration)
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously executes the plugin's main functionality.
    /// </summary>
    public virtual Task ExecuteAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously starts the plugin's operations.
    /// </summary>
    public virtual Task OnStartAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously stops the plugin's operations.
    /// </summary>
    public virtual Task OnStopAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously disposes of the plugin's resources.
    /// </summary>
    public virtual Task OnDisposeAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously updates the game logic during each frame.
    /// </summary>
    public virtual Task UpdateAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Asynchronously renders the game visuals during each frame.
    /// </summary>
    public virtual Task RenderAsync()
    {
        return Task.CompletedTask; // Default no-op
    }

    /// <summary>
    ///     Synchronously initializes the plugin. This is a wrapper for <see cref="InitializeAsync" />.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     Calls <see cref="InitializeAsync" /> to ensure compatibility with <see cref="IPlugin" />.
    ///     Includes unwrapping of any <see cref="AggregateException" /> to surface the inner exception.
    /// </remarks>
    public void Initialize(ExpandoObject configuration)
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