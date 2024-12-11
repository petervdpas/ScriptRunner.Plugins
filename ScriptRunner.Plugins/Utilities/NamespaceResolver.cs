using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility for resolving namespaces used in a plugin assembly.
/// </summary>
public static class NamespaceResolver
{
    /// <summary>
    /// Resolves all unique namespaces used by public types in the given assembly.
    /// </summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>A collection of unique namespace strings.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
    public static IEnumerable<string?> ResolveNamespaces(Assembly assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");

        try
        {
            // Get all public types and their namespaces
            var namespaces = assembly.GetTypes()
                .Where(t => t.IsPublic && !string.IsNullOrWhiteSpace(t.Namespace))
                .Select(t => t.Namespace)
                .Distinct();

            return namespaces;
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Handle partial type loading
            var namespaces = ex.Types
                .Where(t => t is { IsPublic: true } && !string.IsNullOrWhiteSpace(t.Namespace))
                .Select(t => t?.Namespace)
                .Distinct();

            return namespaces;
        }
    }
}
