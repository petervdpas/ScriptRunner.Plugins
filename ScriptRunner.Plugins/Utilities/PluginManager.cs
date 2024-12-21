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
    ///     Discovers plugins and extracts metadata.
    /// </summary>
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
    /// Gets the discovered plugins.
    /// </summary>
    /// <returns>A read-only list of plugin paths.</returns>
    public IReadOnlyList<PluginPathModel> GetDiscoveredPlugins() => _allPlugins.AsReadOnly();
    
    /// <summary>
    ///     Adds namespaces and references for discovered plugins.
    /// </summary>
    public void AddNamespacesAndReferences(
        IEnumerable<string> activePluginNames,
        List<MetadataReference> additionalReferences,
        HashSet<string?> imports,
        string[]? excludedPrefixes = null)
    {
        excludedPrefixes ??= ["FxResources", "System.Private", "Internal"];
        var activePluginSet = new HashSet<string>(activePluginNames, StringComparer.OrdinalIgnoreCase);
        var exceptions = new List<Exception>();

        foreach (var plugin in _allPlugins
                     .Select(pluginPath => pluginPath.GetTuple())
                     .Where(plugin => activePluginSet.Contains(plugin.DllName)))
        {
            // Check if the plugin is already processed
            if (_processedReferences.ContainsKey(plugin.DllName))
            {
                _logger.LogDebug("Plugin already processed: {PluginName}. Skipping.", plugin.DllName);
                continue;
            }

            try
            {
                var assembly = Assembly.LoadFrom(plugin.FullPath);

                additionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
                _logger.LogDebug("Added assembly reference for plugin: {PluginName}", plugin.DllName);

                AddNamespacesFromAssembly(assembly, imports, _logger, excludedPrefixes);

                _processedReferences[plugin.DllName] = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process plugin: {PluginName}", plugin.DllName);
                exceptions.Add(new Exception($"Error processing plugin {plugin.DllName}: {ex.Message}", ex));
            }
        }

        if (exceptions.Count > 0)
            throw new AggregateException("One or more plugins failed to process.", exceptions);
    }

    /// <summary>
    ///     Extracts the main DLL from the .deps.json file.
    /// </summary>
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
    ///     Adds namespaces from an assembly.
    /// </summary>
    private static void AddNamespacesFromAssembly(
        Assembly assembly,
        HashSet<string?> imports,
        ILogger? logger,
        string[] excludedPrefixes)
    {
        try
        {
            var types = assembly.GetTypes();
            logger?.LogDebug("Loaded {TypeCount} types from assembly: {AssemblyName}", types.Length, assembly.FullName);

            var validNamespaces = types
                .Select(t => t.Namespace)
                .Where(ns => !string.IsNullOrWhiteSpace(ns) && IsRelevantNamespace(ns, excludedPrefixes))
                .Distinct();

            foreach (var ns in validNamespaces)
            {
                if (imports.Add(ns))
                {
                    logger?.LogDebug("Added namespace: {Namespace}", ns);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            logger?.LogError(ex, "Failed to load some types from assembly: {AssemblyName}", assembly.FullName);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Unexpected error processing assembly: {AssemblyName}", assembly.FullName);
        }
    }

    private static bool IsRelevantNamespace(string? @namespace, string[] excludedPrefixes)
    {
        return !string.IsNullOrWhiteSpace(@namespace) && !excludedPrefixes.Any(@namespace.StartsWith);
    }
}