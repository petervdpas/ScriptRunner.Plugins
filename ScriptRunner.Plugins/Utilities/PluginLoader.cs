using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptRunner.Plugins.Exceptions;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility for discovering and validating plugins in assemblies.
/// </summary>
public class PluginLoader : IPluginLoader
{
    private readonly IPluginValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoader"/> class.
    /// </summary>
    /// <param name="validator">The plugin validator to use for validating discovered plugins.</param>
    public PluginLoader(IPluginValidator validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    
    /// <summary>
    /// Discovers and validates all plugins in the provided assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for plugins.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Type}"/> containing all valid plugin types that implement <see cref="IPlugin"/>.
    /// </returns>
    /// <exception cref="PluginInitializationException">Thrown if any plugin fails validation.</exception>
    public IEnumerable<Type> DiscoverAndValidatePlugins(Assembly assembly)
    {
        // Materialize the enumerable into a list to prevent multiple enumeration
        var pluginTypes = DiscoverPlugins(assembly).ToList();

        // Validate each plugin
        foreach (var pluginType in pluginTypes)
        {
            _validator.Validate(pluginType);
        }

        return pluginTypes;
    }

    /// <summary>
    /// Discovers all plugins in the provided assembly that implement the <see cref="IPlugin"/> interface.
    /// </summary>
    /// <param name="assembly">The assembly to scan for plugins.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Type}"/> containing all discovered plugin types that implement <see cref="IPlugin"/>.
    /// </returns>
    public IEnumerable<Type> DiscoverPlugins(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
    }
}