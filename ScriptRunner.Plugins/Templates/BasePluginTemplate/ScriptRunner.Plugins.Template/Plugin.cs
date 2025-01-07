using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.Attributes;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.Template;

/// <summary>
///     A plugin that registers and provides ...
/// </summary>
/// <remarks>
///     This plugin demonstrates how to ...
/// </remarks>
[PluginMetadata(
    "Your Plugin Name",
    "A plugin that provides...",
    "YourName",
    "1.0.0",
    PluginSystemConstants.CurrentPluginSystemVersion,
    PluginSystemConstants.CurrentFrameworkVersion,
    [])]
public class Plugin : BasePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    public override string Name => "Your Plugin Name";
    
    /// <summary>
    ///     Initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        base.Initialize(configuration); // Stores settings in LocalStorage by default

        // Use LocalStorage directly for internal logic
        var someValue = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "SomeKey");
        Console.WriteLine($"Retrieved SomeKey: {someValue}");
    }

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    public override void Execute()
    {
        // Access LocalStorage for plugin behavior
        var pluginName = PluginSettingsHelper.RetrieveSetting<string>(LocalStorage, "PluginName") ?? "Unknown";
        Console.WriteLine($"Executing plugin: {pluginName}");

        // Example: Passing LocalStorage to a service
        /*
        var service = new MyService(this); // Pass plugin as ILocalStorageConsumer
        service.DoSomething();
        */
    }
}