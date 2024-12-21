using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Manages discovery, tracking, and namespace/reference extraction for plugins.
/// </summary>
public class PluginManager : IPluginManager
{
    private readonly ConcurrentDictionary<string, bool> _processedReferences = new();
    private readonly List<PluginPathModel> _allPlugins = [];
    private readonly ILogger<PluginManager> _logger;
    private readonly string _pluginRootDirectory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginManager" /> class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostics.</param>
    /// <param name="pluginRootDirectory">Root directory containing plugins.</param>
    public PluginManager(ILogger<PluginManager> logger, string pluginRootDirectory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pluginRootDirectory = pluginRootDirectory ?? throw new ArgumentNullException(nameof(pluginRootDirectory));
    }

    /// <summary>
    ///     Discovers plugins by scanning the root directory and extracting metadata.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">
    ///     Thrown if the plugin root directory does not exist.
    /// </exception>
    public void DiscoverPlugins()
    {
        if (!Directory.Exists(_pluginRootDirectory))
            throw new DirectoryNotFoundException($"Plugin root directory not found: {_pluginRootDirectory}");

        foreach (var pluginDir in Directory.GetDirectories(_pluginRootDirectory))
            try
            {
                var depsJsonFile = Directory.GetFiles(pluginDir, "*.deps.json").FirstOrDefault();
                if (depsJsonFile == null)
                {
                    _logger.LogWarning("No .deps.json file found in plugin directory: {PluginDir}", pluginDir);
                    continue;
                }

                var mainPluginDll = GetPluginDllFromDepsJson(depsJsonFile);
                if (mainPluginDll == null)
                {
                    _logger.LogWarning("No main DLL specified in .deps.json file: {DepsJson}", depsJsonFile);
                    continue;
                }

                _allPlugins.Add(new PluginPathModel(Path.GetFileName(mainPluginDll), mainPluginDll));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process plugin directory: {PluginDir}", pluginDir);
            }
    }

    /// <summary>
    ///     Gets the list of discovered plugins.
    /// </summary>
    /// <returns>A read-only list of <see cref="PluginPathModel"/> representing the discovered plugins.</returns>
    public IReadOnlyList<PluginPathModel> GetDiscoveredPlugins() => _allPlugins.AsReadOnly();
    
    /// <summary>
    ///     Adds namespaces and references for discovered plugins to the specified collections.
    /// </summary>
    /// <param name="activePluginNames">
    ///     A collection of active plugin names to filter the plugins to be processed.
    /// </param>
    /// <param name="additionalReferences">
    ///     A list to store metadata references for the discovered plugins.
    /// </param>
    /// <param name="imports">
    ///     A set to store namespaces from the discovered plugins.
    /// </param>
    /// <param name="excludedPrefixes">
    ///     An optional array of namespace prefixes to exclude.
    /// </param>
    public void AddNamespacesAndReferencesForPlugins(
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        string[]? excludedPrefixes = null)
    {
        excludedPrefixes ??= ["FxResources", "System.Private", "Internal", "Microsoft.Internal"];
        var activePluginSet = new HashSet<string>(activePluginNames, StringComparer.OrdinalIgnoreCase);

        foreach (var plugin in _allPlugins
                     .Select(pluginPath => pluginPath.GetTuple())
                     .Where(plugin => activePluginSet.Contains(plugin.DllName)))
        {
            if (_processedReferences.ContainsKey(plugin.FullPath))
            {
                _logger.LogDebug("Plugin already processed: {PluginPath}. Skipping.", plugin.FullPath);
                continue;
            }

            try
            {
                var assembly = Assembly.LoadFrom(plugin.FullPath);

                if (additionalReferences.All(r => r.Display != assembly.Location))
                {
                    additionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                    _logger.LogDebug("Added assembly reference for plugin: {PluginName}", plugin.DllName);
                }

                AddNamespacesFromAssembly(assembly, imports, excludedPrefixes);

                _processedReferences[plugin.FullPath] = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process plugin: {PluginName}", plugin.DllName);
            }
        }
    }

    /// <summary>
    ///     Extracts the main DLL file path from a .deps.json file.
    /// </summary>
    /// <param name="depsJsonFile">The path to the .deps.json file.</param>
    /// <returns>
    ///     The path to the main DLL file specified in the .deps.json file, or <c>null</c> if not found.
    /// </returns>
    private string? GetPluginDllFromDepsJson(string depsJsonFile)
    {
        try
        {
            var depsContent = File.ReadAllText(depsJsonFile);
            var depsDocument = JsonDocument.Parse(depsContent);

            if (depsDocument.RootElement.TryGetProperty("targets", out var targetsElement))
                foreach (var package in targetsElement.EnumerateObject()
                             .SelectMany(target => target.Value.EnumerateObject()))
                    if (package.Value.TryGetProperty("runtime", out var runtimeElement))
                        foreach (var runtimeFile in runtimeElement.EnumerateObject())
                            if (runtimeFile.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                            {
                                var pluginDllName = runtimeFile.Name;
                                var pluginDir = Path.GetDirectoryName(depsJsonFile);

                                if (pluginDir == null) return null;
                                var pluginDllPath = Path.Combine(pluginDir, pluginDllName);

                                if (File.Exists(pluginDllPath)) return pluginDllPath;
                            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing .deps.json file: {DepsJsonFile}", depsJsonFile);
        }

        return null;
    }

    /// <summary>
    ///     Adds namespaces from an assembly to the specified imports collection.
    /// </summary>
    /// <param name="assembly">The assembly to process.</param>
    /// <param name="imports">The collection to store extracted namespaces.</param>
    /// <param name="excludedPrefixes">
    ///     An array of namespace prefixes to exclude from the imports collection.
    /// </param>
    private void AddNamespacesFromAssembly(
        Assembly assembly,
        HashSet<string?> imports,
        string[] excludedPrefixes)
    {
        try
        {
            var types = assembly.GetTypes();
            _logger.LogDebug("Loaded {TypeCount} types from assembly: {AssemblyName}", types.Length, assembly.FullName);

            foreach (var ns in types
                         .Select(t => t.Namespace)
                         .Where(ns => !string.IsNullOrWhiteSpace(ns) && IsRelevantNamespace(ns, excludedPrefixes))
                         .Distinct())
            {
                if (imports.Add(ns))
                {
                    _logger.LogDebug("Added namespace: {Namespace}", ns);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            _logger.LogError(ex, "Failed to load some types from assembly: {AssemblyName}", assembly.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing assembly: {AssemblyName}", assembly.FullName);
        }
    }

    /// <summary>
    ///     Determines whether a namespace is relevant for inclusion based on the exclusion rules.
    /// </summary>
    /// <param name="namespace">The namespace to evaluate.</param>
    /// <param name="excludedPrefixes">An array of namespace prefixes to exclude.</param>
    /// <returns>
    ///     <c>true</c> if the namespace is relevant; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsRelevantNamespace(string? @namespace, string[] excludedPrefixes)
    {
        return !string.IsNullOrWhiteSpace(@namespace) && !excludedPrefixes.Any(@namespace.StartsWith);
    }
}