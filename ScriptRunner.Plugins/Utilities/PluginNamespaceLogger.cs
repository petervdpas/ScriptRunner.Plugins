using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility to log namespaces used by a plugin assembly.
/// </summary>
public static class PluginNamespaceLogger
{
    /// <summary>
    /// Logs namespaces used by the plugin assembly.
    /// </summary>
    /// <param name="assembly">The plugin assembly to analyze.</param>
    /// <param name="logger">The logger instance for logging namespace information.</param>
    public static void LogPluginNamespaces(Assembly assembly, ILogger logger)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");
        
        if (logger == null)
            throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");

        var namespaces = NamespaceResolver.ResolveNamespaces(assembly);

        foreach (var ns in namespaces)
        {
            logger.LogInformation("Namespace discovered: {Namespace}", ns);
        }
    }
}