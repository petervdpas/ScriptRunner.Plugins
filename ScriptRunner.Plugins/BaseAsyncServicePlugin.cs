using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous service plugins.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous service plugins by providing
///     default asynchronous behavior for initialization and service registration.
/// </remarks>
public abstract class BaseAsyncServicePlugin : IAsyncServicePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    public abstract string Name { get; }

    /// <summary>
    ///     Asynchronously registers the plugin's services into the provided DI container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous registration operation.</returns>
    public virtual Task RegisterServicesAsync(IServiceCollection services)
    {
        // Default implementation: Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous initialization operation.</returns>
    public virtual Task InitializeAsync(IDictionary<string, object> configuration)
    {
        // Default implementation: Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously executes the plugin's main functionality.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous execution operation.</returns>
    public virtual Task ExecuteAsync()
    {
        // Default implementation: Do nothing
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Synchronously initializes the plugin. This is a wrapper for <see cref="InitializeAsync" />.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     Calls <see cref="InitializeAsync" /> to ensure compatibility with <see cref="IPlugin" />.
    ///     Includes unwrapping of any <see cref="AggregateException" /> to surface the inner exception.
    /// </remarks>
    public void Initialize(IDictionary<string, object> configuration)
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