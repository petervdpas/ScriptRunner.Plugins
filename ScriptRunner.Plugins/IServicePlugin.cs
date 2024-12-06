using Microsoft.Extensions.DependencyInjection;

namespace ScriptRunner.Plugins;

/// <summary>
/// Represents a plugin that can register services into the application's dependency injection container.
/// </summary>
public interface IServicePlugin : IPlugin
{
    /// <summary>
    /// Registers the plugin's services into the provided DI container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    void RegisterServices(IServiceCollection services);
}