using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Represents a service for discovering and validating plugins in assemblies.
/// </summary>
public interface IPluginLoader
{
    /// <summary>
    /// Discovers and validates all plugins in the provided assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for plugins.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Type}"/> containing all valid plugin types that implement <see cref="IPlugin"/>.
    /// </returns>
    IEnumerable<Type> DiscoverAndValidatePlugins(Assembly assembly);

    /// <summary>
    /// Discovers all plugins in the provided assembly that implement the <see cref="IPlugin"/> interface.
    /// </summary>
    /// <param name="assembly">The assembly to scan for plugins.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Type}"/> containing all discovered plugin types that implement <see cref="IPlugin"/>.
    /// </returns>
    IEnumerable<Type> DiscoverPlugins(Assembly assembly);
}