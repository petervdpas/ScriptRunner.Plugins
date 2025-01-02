using System.Collections.Generic;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins;

/// <summary>
///     Provides a base implementation for game plugins that support lifecycle management and frame-based updates and
///     rendering.
/// </summary>
/// <remarks>
///     This abstract class simplifies the creation of game plugins by providing default no-op implementations for
///     lifecycle and frame-based methods, allowing derived classes to override only the methods relevant to their
///     behavior.
/// </remarks>
public abstract class BaseGamePlugin : IGamePlugin
{
    /// <summary>
    ///     Gets the name of the plugin.
    /// </summary>
    /// <value>A string representing the name of the plugin.</value>
    public abstract string Name { get; }

    /// <summary>
    ///     Initializes the plugin with the specified configuration.
    /// </summary>
    /// <param name="configuration">A dictionary containing configuration key-value pairs for the plugin.</param>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to perform
    ///     initialization tasks, such as setting up game resources or configurations.
    /// </remarks>
    public virtual void Initialize(IEnumerable<PluginSettingDefinition> configuration)
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Executes the plugin's main functionality.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to define
    ///     the plugin's primary behavior.
    /// </remarks>
    public virtual void Execute()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin starts its operations.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to
    ///     handle game startup logic, such as initializing game objects or setting the initial state.
    /// </remarks>
    public virtual void OnStart()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin stops its operations.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to
    ///     handle game shutdown logic, such as saving the state or stopping timers.
    /// </remarks>
    public virtual void OnStop()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Called when the plugin is disposed and its resources are cleaned up.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to
    ///     release resources, such as graphics assets or event subscriptions.
    /// </remarks>
    public virtual void OnDispose()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Updates the game logic during each frame of the game loop.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to
    ///     handle game logic, such as player input, AI behavior, or collision detection.
    /// </remarks>
    public virtual void Update()
    {
        // Default implementation: Do nothing
    }

    /// <summary>
    ///     Renders the game visuals during each frame of the game loop.
    /// </summary>
    /// <remarks>
    ///     This method provides a default no-op implementation. Derived classes can override it to
    ///     handle game rendering, such as drawing characters, backgrounds, or UI elements.
    /// </remarks>
    public virtual void Render()
    {
        // Default implementation: Do nothing
    }
}