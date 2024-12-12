using System;
using System.Collections.Generic;
using ScriptRunner.Plugins.Attributes;

namespace ScriptRunner.Plugins.Template;

/// <summary>
///     A plugin that registers and provides ...
/// </summary>
/// <remarks>
///     This plugin demonstrates how to ...
/// </remarks>
[PluginMetadata(
    name: "Your Plugin Name",
    description: "A plugin that provides...",
    author: "YourName",
    version: "1.0.0",
    pluginSystemVersion: "1.0.24", // Check this!!!
    frameworkVersion: ".NET 8.0",
    services: [])]
public class Plugin : BasePlugin
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public override string Name => "Your Plugin Name";

    /// <summary>
    /// Initializes the plugin using the provided configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    public override void Initialize(IDictionary<string, object> configuration)
    {
        Console.WriteLine(configuration.TryGetValue("SomeKey", out var someValue)
            ? $"SomeKey value: {someValue}"
            : "SomeKey not found in configuration.");
    }
    
    /// <summary>
    /// Executes the plugin's main functionality.
    /// </summary>
    public override void Execute()
    {
        Console.WriteLine("Your Plugin executed.");
    }
}