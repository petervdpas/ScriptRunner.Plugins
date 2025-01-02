using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for extracting namespaces from assemblies.
/// </summary>
public static class NamespaceTool
{
    /// <summary>
    ///     Extracts namespaces from the given assembly and adds them to the provided imports set.
    /// </summary>
    /// <param name="assembly">The assembly to extract namespaces from.</param>
    /// <param name="imports">A set to store the namespaces.</param>
    /// <param name="logger">An optional logger instance for diagnostic logging.</param>
    /// <param name="excludedPrefixes">Optional prefixes of namespaces to exclude. Defaults to common internal namespaces.</param>
    public static void ExtractNamespaces(
        Assembly assembly,
        HashSet<string?> imports,
        ILogger? logger,
        string[]? excludedPrefixes = null)
    {
        excludedPrefixes ??= ["FxResources", "System.Private", "Internal", "Microsoft.Internal"];

        try
        {
            // Get all types in the assembly
            var types = assembly.GetTypes();
            logger?.LogDebug("Loaded {TypeCount} types from assembly: {AssemblyName}", types.Length, assembly.FullName);

            // Filter and add namespaces to the imports set
            foreach (var ns in types
                         .Select(t => t.Namespace)
                         .Where(ns => !string.IsNullOrWhiteSpace(ns) && IsRelevantNamespace(ns, excludedPrefixes))
                         .Distinct())
                if (imports.Add(ns))
                    logger?.LogDebug("Added namespace: {Namespace}", ns);
        }
        catch (ReflectionTypeLoadException ex)
        {
            logger?.LogError(ex, "Failed to load some types from assembly: {AssemblyName}", assembly.FullName);
            foreach (var loaderException in ex.LoaderExceptions)
                logger?.LogError(loaderException, "Loader exception encountered.");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error processing assembly: {AssemblyName}", assembly.FullName);
        }
    }

    /// <summary>
    ///     Determines if a namespace is relevant for inclusion based on the excluded prefixes.
    /// </summary>
    /// <param name="namespace">The namespace to evaluate.</param>
    /// <param name="excludedPrefixes">An array of namespace prefixes to exclude.</param>
    /// <returns><c>true</c> if the namespace is relevant; otherwise, <c>false</c>.</returns>
    private static bool IsRelevantNamespace(string? @namespace, string[] excludedPrefixes)
    {
        return !string.IsNullOrWhiteSpace(@namespace) && !excludedPrefixes.Any(@namespace.StartsWith);
    }
}