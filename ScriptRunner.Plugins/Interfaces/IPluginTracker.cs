using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface defining the contract for tracking plugins and their dependencies.
/// </summary>
public interface IPluginTracker
{
    /// <summary>
    ///     Sets libraries to be skipped during dependency validation.
    /// </summary>
    /// <param name="skipLibraryChecks">Array of library names to skip.</param>
    void SetSkipLibraries(IEnumerable<string>? skipLibraryChecks);

    /// <summary>
    /// Discovers plugins in the specified root directory, extracts their metadata,
    /// and tracks their related dependencies.
    /// </summary>
    void DiscoverAndTrackPlugins();

    /// <summary>
    /// Returns all tracked main plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing main plugin DLLs.</returns>
    List<DependencyModel> GetTrackedPlugins();

    /// <summary>
    /// Returns all tracked dependency DLLs (excluding main plugins).
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing dependency DLLs.</returns>
    List<DependencyModel> GetTrackedDependencies();

    /// <summary>
    /// Loads a dependency DLL into an isolated plugin context and tracks it.
    /// </summary>
    /// <param name="directory">The directory where the DLL is located.</param>
    /// <param name="dllName">The name of the DLL to load.</param>
    void LoadDependency(string directory, string dllName);
}