namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Defines a contract for plugins that consume a local storage instance.
/// </summary>
/// <remarks>
///     Implement this interface to allow a plugin to receive an <see cref="ILocalStorage" /> instance
///     during its initialization or lifecycle. This enables the plugin to dynamically store and retrieve
///     data throughout its execution.
/// </remarks>
public interface ILocalStorageConsumer
{
    /// <summary>
    ///     Sets the local storage instance for the plugin.
    /// </summary>
    /// <param name="localStorage">The <see cref="ILocalStorage" /> instance to associate with the plugin.</param>
    /// <remarks>
    ///     This method is typically called by the plugin host to inject the <see cref="ILocalStorage" /> instance.
    ///     Implementing classes can use this instance to store and manage their configuration, state, or runtime data.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if the <paramref name="localStorage" /> argument is <c>null</c>.
    /// </exception>
    void SetLocalStorage(ILocalStorage localStorage);

    /// <summary>
    ///     Gets the local storage instance associated with the plugin.
    /// </summary>
    /// <returns>The <see cref="ILocalStorage" /> instance associated with the plugin.</returns>
    /// <remarks>
    ///     This method provides access to the local storage instance for internal plugin services that
    ///     require dynamic state or configuration management.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the local storage has not been set before this method is called.
    /// </exception>
    ILocalStorage GetLocalStorage();
}
