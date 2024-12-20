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
    /// Gets all tracked plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing main plugin DLLs.</returns>
    List<DependencyModel> GetTrackedPlugins();
}