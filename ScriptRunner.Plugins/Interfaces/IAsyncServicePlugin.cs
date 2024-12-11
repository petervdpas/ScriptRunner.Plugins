using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Represents an asynchronous plugin that can register services into the application's dependency injection container.
/// </summary>
/// <remarks>
/// Plugins implementing this interface support asynchronous service registration,
/// enabling non-blocking operations for tasks such as remote configuration loading or dynamic dependency initialization.
/// </remarks>
public interface IAsyncServicePlugin : IAsyncPlugin
{
    /// <summary>
    /// Asynchronously registers the plugin's services into the provided DI container.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous registration operation.</returns>
    /// <remarks>
    /// This method should be used to add services to the dependency injection container.
    /// It supports asynchronous operations, such as fetching external configurations or initializing connections.
    /// </remarks>
    Task RegisterServicesAsync(IServiceCollection services);
}