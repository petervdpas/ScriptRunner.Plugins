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

    /// <summary>
    /// Adds namespaces and metadata references for active plugins to the specified collections.
    /// </summary>
    /// <param name="activePluginNames">A collection of active plugin names to filter the plugins to be processed.</param>
    /// <param name="additionalReferences">A list to store metadata references for the discovered plugins.</param>
    /// <param name="imports">A set to store namespaces extracted from the discovered plugins.</param>
    /// <param name="excludedPrefixes">
    /// Optional. An array of namespace prefixes to exclude from the imports collection. Default exclusions will be used if not provided.
    /// </param>
    void AddNamespacesAndReferencesForPlugins(
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        string[]? excludedPrefixes = null);
}