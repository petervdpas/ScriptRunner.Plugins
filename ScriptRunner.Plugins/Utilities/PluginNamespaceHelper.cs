using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Attributes;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Helper for extracting and managing plugin namespaces and references.
/// </summary>
public static class PluginNamespaceHelper
{
    /// <summary>
    /// Adds references and namespaces for active plugins.
    /// </summary>
    /// <param name="discoveredPlugins">A collection of discovered plugins.</param>
    /// <param name="activePluginNames">A list of active plugin names.</param>
    /// <param name="additionalReferences">A list to store references for active plugins.</param>
    /// <param name="imports">A set to store namespaces for active plugins.</param>
    /// <param name="logger">An optional logger instance for diagnostics.</param>
    public static void AddPluginReferencesAndNamespaces(
        IEnumerable<(string DllPath, PluginMetadataAttribute Metadata)> discoveredPlugins,
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        ILogger? logger = null)
    {
        var activePluginSet = new HashSet<string>(activePluginNames, StringComparer.OrdinalIgnoreCase);
        var exceptions = new List<Exception>();

        foreach (var (dllPath, metadata) in discoveredPlugins)
        {
            if (!activePluginSet.Contains(metadata.Name))
                continue;

            try
            {
                var assembly = Assembly.LoadFrom(dllPath);

                // Add references
                additionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                logger?.LogDebug("Added assembly reference for plugin: {PluginName}", metadata.Name);

                // Extract namespaces
                AddNamespacesFromAssembly(assembly, imports, logger);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to process active plugin: {PluginName}", metadata.Name);
                exceptions.Add(new Exception($"Error processing plugin {metadata.Name}: {ex.Message}", ex));
            }
        }
        
        if (exceptions.Count > 0)
            throw new AggregateException("One or more plugins failed to process.", exceptions);
    }

    /// <summary>
    /// Recursively adds namespaces from an assembly and its dependencies.
    /// </summary>
    /// <param name="assembly">The assembly to process.</param>
    /// <param name="imports">A set to store namespaces.</param>
    /// <param name="logger">An optional logger instance for diagnostics.</param>
    private static void AddNamespacesFromAssembly(
        Assembly assembly,
        HashSet<string?> imports,
        ILogger? logger = null)
    {
        var exceptions = new List<Exception>();

        // Extract namespaces from the current assembly
        var namespaces = assembly.GetTypes()
            .Select(t => t.Namespace)
            .Where(ns => !string.IsNullOrWhiteSpace(ns))
            .Distinct();

        foreach (var ns in namespaces)
        {
            if (ns != null && imports.Add(ns))
                logger?.LogDebug("Added namespace: {Namespace}", ns);
        }

        // Process dependencies
        foreach (var dependencyName in assembly.GetReferencedAssemblies())
        {
            try
            {
                var dependencyAssembly = Assembly.Load(dependencyName);
                AddNamespacesFromAssembly(dependencyAssembly, imports, logger);
            }
            catch (Exception ex)
            {
                logger?.LogWarning("Failed to load dependency: {DependencyName}", dependencyName.FullName);
                exceptions.Add(new Exception($"Error loading dependency {dependencyName.FullName}: {ex.Message}", ex));
            }
        }
        
        if (exceptions.Count > 0)
            throw new AggregateException("One or more dependencies failed to load namespaces.", exceptions);
    }
}