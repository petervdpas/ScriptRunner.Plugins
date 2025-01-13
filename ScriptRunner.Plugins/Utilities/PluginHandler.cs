using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Tools;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods to initialize and execute plugins in an application.
/// </summary>
public static class PluginHandler
{
    /// <summary>
    /// Initializes a plugin asynchronously with the provided configuration and services.
    /// </summary>
    /// <param name="plugin">The plugin instance to initialize and execute.</param>
    /// <param name="configuration">
    /// A collection of <see cref="PluginSettingDefinition"/> objects containing the plugin's configuration settings.
    /// </param>
    /// <param name="serviceCollection">The dependency injection service collection for registering plugin services.</param>
    /// <remarks>
    /// This method determines the type of the plugin (e.g., asynchronous, service-based, or standard) and initializes it
    /// accordingly. For asynchronous plugins, their initialization and execution methods are awaited. For service plugins,
    /// any services provided by the plugin are registered into the provided <paramref name="serviceCollection"/>.
    /// </remarks>
    public static async Task InitPluginAsync(
        IPlugin plugin,
        IEnumerable<PluginSettingDefinition> configuration,
        IServiceCollection serviceCollection)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(serviceCollection);

        // Handle LocalStorageConsumer logic
        if (plugin is ILocalStorageConsumer localStorageConsumer)
        {
            var localStorage = new LocalStorage();
            if (localStorage == null) throw new InvalidOperationException("LocalStorage instance is not initialized.");
            localStorageConsumer.SetLocalStorage(localStorage);
        }

        // Determine plugin-type and handle initialization, execution, and lifecycle management
        switch (plugin)
        {
            case IAsyncServicePlugin asyncServicePlugin:
                await asyncServicePlugin.RegisterServicesAsync(serviceCollection);
                await asyncServicePlugin.InitializeAsync(configuration);
                await asyncServicePlugin.ExecuteAsync();
                break;

            case IAsyncPlugin asyncPlugin:
                await asyncPlugin.InitializeAsync(configuration);
                await asyncPlugin.ExecuteAsync();
                break;

            case IServicePlugin servicePlugin:
                servicePlugin.RegisterServices(serviceCollection);
                servicePlugin.Initialize(configuration);
                servicePlugin.Execute();
                break;

            default:
                plugin.Initialize(configuration);
                plugin.Execute();
                break;
        }
    }
}