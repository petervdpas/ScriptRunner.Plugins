namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a game plugin that supports lifecycle management and frame-based updates and rendering.
/// </summary>
/// <remarks>
///     Plugins implementing this interface follow the game development pattern, where game logic
///     and visuals are updated on a per-frame basis. It extends the <see cref="ILifecyclePlugin" />
///     interface, inheriting lifecycle management methods for start, stop, and cleanup phases.
/// </remarks>
public interface IGamePlugin : ILifecyclePlugin
{
    /// <summary>
    ///     Updates the game logic during each frame of the game loop.
    /// </summary>
    /// <remarks>
    ///     This method is called continuously as part of the game loop and is responsible for handling
    ///     game logic such as player movement, enemy AI, collision detection, and score updates.
    /// </remarks>
    void Update();

    /// <summary>
    ///     Renders the game visuals during each frame of the game loop.
    /// </summary>
    /// <remarks>
    ///     This method is called continuously as part of the game loop after the <see cref="Update" /> method.
    ///     It is responsible for drawing the game scene, including players, enemies, backgrounds, and UI elements.
    /// </remarks>
    void Render();
}