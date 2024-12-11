using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins;

/// <summary>
/// Provides a base implementation for plugins that register services in the application's dependency injection container.
/// </summary>
/// <remarks>
/// This abstract class simplifies the creation of service-based plugins by providing default behavior for initialization
/// and execution, while requiring derived classes to specify their name and service registrations.
/// </remarks>
public abstract class BaseServicePlugin : IServicePlugin
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    /// <value>
    /// A string representing the name of the plugin.
    /// </value>
    /// <remarks>
    /// Derived classes must implement this property to provide a unique name for the plugin.
    /// </remarks>
    public abstract string Name { get; }

    /// <summary>
    /// Registers the plugin's services in the provided dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the plugin's services should be added.</param>
    /// <remarks>
    /// Derived classes must implement this method to define the services required by the plugin.
    /// </remarks>
    public abstract void RegisterServices(IServiceCollection services);

    /// <summary>
    /// Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">
    /// A dictionary containing configuration key-value pairs for the plugin.
    /// </param>
    /// <remarks>
    /// This method provides a default no-op implementation. Derived classes can override it to perform
    /// initialization tasks such as reading configuration values or setting up internal state.
    /// </remarks>
    public virtual void Initialize(IDictionary<string, object> configuration) { }

    /// <summary>
    /// Executes the plugin's main functionality.
    /// </summary>
    /// <remarks>
    /// This method provides a default no-op implementation. Derived classes can override it to define
    /// the plugin's primary behavior.
    /// </remarks>
    public virtual void Execute() { }
}