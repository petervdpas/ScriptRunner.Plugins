using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Manages discovery, tracking, and namespace/reference extraction for plugins.
/// </summary>
public class PluginManager : IPluginManager
{
    private readonly List<PluginPathModel> _allPlugins = [];
    private readonly ILogger<PluginManager> _logger;
    private readonly string _pluginRootDirectory;
    private readonly ConcurrentDictionary<string, bool> _processedReferences = new();

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
    /// <returns>A read-only list of <see cref="PluginPathModel" /> representing the discovered plugins.</returns>
    public IReadOnlyList<PluginPathModel> GetDiscoveredPlugins()
    {
        return _allPlugins.AsReadOnly();
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
}