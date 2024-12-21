using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface for managing plugin discovery, tracking, and namespace/reference extraction.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Discovers plugins by scanning the root directory and extracting metadata from available plugins.
    /// </summary>
    /// <exception cref="System.IO.DirectoryNotFoundException">
    /// Thrown if the plugin root directory does not exist.
    /// </exception>
    void DiscoverPlugins();

    /// <summary>
    /// Retrieves the list of plugins that have been discovered.
    /// </summary>
    /// <returns>A read-only list of <see cref="PluginPathModel"/> objects representing discovered plugins.</returns>
    IReadOnlyList<PluginPathModel> GetDiscoveredPlugins();
}