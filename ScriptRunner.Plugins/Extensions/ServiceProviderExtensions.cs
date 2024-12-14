using System;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IServiceProvider" /> to simplify service resolution.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    ///     Resolves the specified service type from the <see cref="IServiceProvider" /> and ensures that it is not null.
    /// </summary>
    /// <typeparam name="T">The type of the service to resolve.</typeparam>
    /// <param name="serviceProvider">The service provider instance to resolve the service from.</param>
    /// <returns>An instance of the requested service type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the requested service type is not registered or cannot be
    ///     resolved.
    /// </exception>
    /// <remarks>
    ///     This extension method simplifies service resolution by avoiding the need to cast the result manually.
    ///     It ensures that a meaningful exception is thrown if the service is not registered.
    /// </remarks>
    public static T ResolveRequiredService<T>(this IServiceProvider serviceProvider) where T : class
    {
        if (serviceProvider.GetService(typeof(T)) is not T service)
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");

        return service;
    }
}