using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface for managing plugin discovery, tracking, and namespace/reference extraction.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Discovers plugins in the root directory and extracts metadata.
    /// </summary>
    void DiscoverPlugins();

    /// <summary>
    /// Adds namespaces and references for active plugins.
    /// </summary>
    /// <param name="activePluginNames">The names of active plugins to process.</param>
    /// <param name="additionalReferences">A list to store additional metadata references.</param>
    /// <param name="imports">A set to store namespaces for active plugins.</param>
    /// <param name="excludedPrefixes">
    /// Optional. A list of namespace prefixes to exclude from imports. Default exclusions will be used if not provided.
    /// </param>
    void AddNamespacesAndReferences(
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        string[]? excludedPrefixes = null);
}