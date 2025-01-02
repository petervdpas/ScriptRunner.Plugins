using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for asynchronous plugins, extending <see cref="IAsyncPlugin" />.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of asynchronous plugins by providing default
///     asynchronous behavior for initialization and execution, while requiring derived classes to
///     define a unique name.
/// </remarks>
public abstract class BaseAsyncPlugin : IAsyncPlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    /// <remarks>
    ///     Derived classes must implement this property to provide a unique identifier for the plugin.
    /// </remarks>
    public abstract string Name { get; }

    /// <summary>
    ///     Asynchronously initializes the plugin using the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous initialization operation.</returns>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     asynchronous initialization tasks, such as reading configurations or establishing connections.
    /// </remarks>
    public virtual Task InitializeAsync(ExpandoObject configuration)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously executes the main functionality of the plugin.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous execution operation.</returns>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     the plugin's core asynchronous behavior.
    /// </remarks>
    public virtual Task ExecuteAsync()
    {
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