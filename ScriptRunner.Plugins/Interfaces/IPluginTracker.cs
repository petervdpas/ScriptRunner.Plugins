using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface defining methods for tracking plugins and their dependencies.
/// </summary>
public interface IPluginTracker
{
    /// <summary>
    /// Discovers plugins in the specified root directory, extracts their metadata,
    /// and tracks their related dependencies.
    /// </summary>
    void DiscoverAndTrackPlugins();

    /// <summary>
    /// Gets all tracked dependencies, including plugins and non-plugin libraries.
    /// </summary>
    /// <returns>A list of all tracked <see cref="DependencyModel"/> objects.</returns>
    List<DependencyModel> GetAllDependencies();

    /// <summary>
    /// Gets all tracked plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing main plugin DLLs.</returns>
    List<DependencyModel> GetTrackedPlugins();

    /// <summary>
    /// Gets all tracked dependencies that are not main plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing dependency DLLs.</returns>
    List<DependencyModel> GetTrackedDependencies();

    /// <summary>
    /// Determines if a file is a native library.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is a native library; otherwise, false.</returns>
    bool IsNativeLibrary(string filePath);
}