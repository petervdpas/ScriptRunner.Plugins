using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Service for discovering plugins and tracking their dependencies.
/// </summary>
public class PluginTracker : IPluginTracker
{
    private readonly List<DependencyModel> _allPlugins = [];
    private readonly ILogger<PluginTracker> _logger;
    private readonly string _pluginRootDirectory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginTracker" /> class.
    /// </summary>
    /// <param name="logger">The logger for tracking operations and errors.</param>
    /// <param name="pluginRootDirectory">The root directory containing plugin subdirectories.</param>
    public PluginTracker(ILogger<PluginTracker> logger, string pluginRootDirectory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pluginRootDirectory = pluginRootDirectory ?? throw new ArgumentNullException(nameof(pluginRootDirectory));
    }

    /// <summary>
    ///     Discovers plugins in the specified root directory by finding their .deps.json files.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">
    ///     Thrown if the plugin root directory does not exist.
    /// </exception>
    public void DiscoverAndTrackPlugins()
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

                _allPlugins.Add(new DependencyModel(Path.GetFileName(mainPluginDll), mainPluginDll));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process plugin directory '{PluginDir}'", pluginDir);
            }
    }

    /// <summary>
    ///     Gets all tracked plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel" /> representing main plugin DLLs.</returns>
    public List<DependencyModel> GetTrackedPlugins()
    {
        return _allPlugins;
    }

    /// <summary>
    ///     Extracts the main DLL from the .deps.json file.
    /// </summary>
    /// <param name="depsJsonFile">The path to the .deps.json file.</param>
    /// <returns>The path to the main plugin DLL if found; otherwise, null.</returns>
    private string? GetPluginDllFromDepsJson(string depsJsonFile)
    {
        try
        {
            var depsContent = File.ReadAllText(depsJsonFile);
            var depsDocument = JsonDocument.Parse(depsContent);

            if (depsDocument.RootElement.TryGetProperty("targets", out var targetsElement))
                foreach (var target in targetsElement.EnumerateObject())
                foreach (var package in target.Value.EnumerateObject())
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