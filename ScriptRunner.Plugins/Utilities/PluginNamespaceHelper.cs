using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Attributes;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Helper for extracting and managing plugin namespaces and references.
/// </summary>
public static class PluginNamespaceHelper
{
    /// <summary>
    ///     Adds references and namespaces for active plugins.
    /// </summary>
    /// <param name="discoveredPlugins">A collection of discovered plugins.</param>
    /// <param name="activePluginNames">A list of active plugin names.</param>
    /// <param name="additionalReferences">A list to store references for active plugins.</param>
    /// <param name="imports">A set to store namespaces for active plugins.</param>
    /// <param name="logger">An optional logger instance for diagnostics.</param>
    /// <param name="excludedPrefixes">
    ///     A list of namespace prefixes to exclude from imports. If null, default exclusions will be used.
    /// </param>
    public static void AddPluginReferencesAndNamespaces(
        IEnumerable<(string DllPath, PluginMetadataAttribute Metadata)> discoveredPlugins,
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        string[]? excludedPrefixes = null,
        ILogger? logger = null)
    {
        excludedPrefixes ??= ["FxResources", "System.Private", "Internal"];
        var activePluginSet = new HashSet<string>(activePluginNames, StringComparer.OrdinalIgnoreCase);

        foreach (var (dllPath, metadata) in discoveredPlugins)
        {
            if (!activePluginSet.Contains(metadata.Name))
                continue;

            try
            {
                var assembly = Assembly.LoadFrom(dllPath);

                // Add metadata reference
                additionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                logger?.LogDebug("Added assembly reference for plugin: {PluginName}", metadata.Name);

                // Extract namespaces
                AddNamespacesFromAssembly(assembly, imports, logger, excludedPrefixes);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to process active plugin: {PluginName}", metadata.Name);
            }
        }
    }

    /// <summary>
    ///     Adds namespaces from an assembly, optionally including its dependencies.
    /// </summary>
    /// <param name="assembly">The assembly to process.</param>
    /// <param name="imports">A set to store namespaces.</param>
    /// <param name="logger">An optional logger instance for diagnostics.</param>
    /// <param name="excludedPrefixes">Namespace prefixes to exclude from imports.</param>
    /// <param name="includeDependencies">Whether to include namespaces from dependencies.</param>
    private static void AddNamespacesFromAssembly(
        Assembly assembly,
        HashSet<string?> imports,
        ILogger? logger,
        string[] excludedPrefixes,
        bool includeDependencies = false)
    {
        try
        {
            // Extract namespaces from the main assembly
            foreach (var ns in assembly.GetTypes()
                         .Select(t => t.Namespace)
                         .Where(ns => !string.IsNullOrWhiteSpace(ns) && IsRelevantNamespace(ns, excludedPrefixes))
                         .Distinct())
                if (imports.Add(ns))
                    logger?.LogDebug("Added namespace: {Namespace}", ns);

            // Optionally include dependencies
            if (!includeDependencies) return;

            foreach (var dependencyName in assembly.GetReferencedAssemblies())
                try
                {
                    var dependencyAssembly = Assembly.Load(dependencyName);
                    AddNamespacesFromAssembly(dependencyAssembly, imports, logger, excludedPrefixes);
                }
                catch (Exception)
                {
                    logger?.LogWarning("Failed to load dependency: {DependencyName}", dependencyName.FullName);
                }
        }
        catch (ReflectionTypeLoadException ex)
        {
            logger?.LogError(ex, "Failed to load some types from assembly: {AssemblyName}", assembly.FullName);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to process assembly: {AssemblyName}", assembly.FullName);
        }
    }

    /// <summary>
    ///     Determines if a namespace is relevant for inclusion.
    /// </summary>
    /// <param name="namespace">The namespace to evaluate.</param>
    /// <param name="excludedPrefixes">Namespace prefixes to exclude.</param>
    /// <returns>True if the namespace is relevant; otherwise, false.</returns>
    private static bool IsRelevantNamespace(string? @namespace, string[] excludedPrefixes)
    {
        return !string.IsNullOrWhiteSpace(@namespace) && !excludedPrefixes.Any(@namespace.StartsWith);
    }
}