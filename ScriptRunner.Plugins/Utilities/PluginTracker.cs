using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Service for discovering plugins and tracking their dependencies.
/// </summary>
public class PluginTracker : IPluginTracker
{
    private readonly List<DependencyModel> _allDependencies = [];

    private readonly string _pluginRootDirectory;
    private readonly string _dependenciesDirectory;

    private readonly ILogger<PluginTracker> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginTracker"/> class.
    /// </summary>
    /// <param name="logger">The logger for tracking operations and errors.</param>
    /// <param name="pluginRootDirectory">The root directory containing plugin subdirectories.</param>
    /// <param name="dependenciesDirectory">The directory name where dependencies are stored.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public PluginTracker(ILogger<PluginTracker> logger, string pluginRootDirectory, string dependenciesDirectory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pluginRootDirectory = pluginRootDirectory ?? throw new ArgumentNullException(nameof(pluginRootDirectory));
        _dependenciesDirectory = dependenciesDirectory ?? throw new ArgumentNullException(nameof(dependenciesDirectory));
    }

    /// <summary>
    /// Discovers plugins in the specified root directory, extracts their metadata,
    /// and tracks their related dependencies.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the plugin root directory does not exist.
    /// </exception>
    public void DiscoverAndTrackPlugins()
    {
        if (!Directory.Exists(_pluginRootDirectory))
            throw new DirectoryNotFoundException($"Plugin root directory not found: {_pluginRootDirectory}");

        foreach (var pluginDir in Directory.GetDirectories(_pluginRootDirectory))
        {
            var pluginDll = Directory.GetFiles(pluginDir, "*.dll").FirstOrDefault();
            if (pluginDll == null)
                continue;

            try
            {
                var dllName = Path.GetFileName(pluginDll);
                _allDependencies.Add(new DependencyModel(dllName, pluginDll, true));
                TrackDependencies(pluginDir, _dependenciesDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process plugin directory '{PluginDir}'", pluginDir);
            }
        }
    }

    /// <summary>
    /// Gets all tracked dependencies, including plugins and non-plugin libraries.
    /// </summary>
    /// <returns>A list of all tracked <see cref="DependencyModel"/> objects.</returns>
    public List<DependencyModel> GetAllDependencies() => _allDependencies;

    /// <summary>
    /// Gets all tracked plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing main plugin DLLs.</returns>
    public List<DependencyModel> GetTrackedPlugins()
    {
        var seenDllNames = new HashSet<string>();

        return _allDependencies
            .Where(d => d.IsPlugin())
            .Where(d => seenDllNames.Add(d.GetTuple().DllName))
            .ToList();
    }

    /// <summary>
    /// Gets all tracked dependencies that are not main plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing dependency DLLs.</returns>
    public List<DependencyModel> GetTrackedDependencies()
    {
        var seenDllNames = new HashSet<string>();

        return _allDependencies
            .Where(d => !d.IsPlugin())
            .Where(d => seenDllNames.Add(d.GetTuple().DllName))
            .ToList();
    }

    /// <summary>
    /// Tracks all DLLs in a plugin directory and its host-defined dependencies directory.
    /// </summary>
    /// <param name="pluginDirectory">The root directory of the plugin.</param>
    /// <param name="dependenciesDirectory">The name of the directory containing plugin dependencies.</param>
    private void TrackDependencies(string pluginDirectory, string dependenciesDirectory)
    {
        var fullDependenciesDir = Path.Combine(pluginDirectory, dependenciesDirectory);
        if (Directory.Exists(fullDependenciesDir))
        {
            TrackDllsInDirectory(fullDependenciesDir, false);
        }
        else
        {
            _logger.LogWarning("Dependencies directory not found: {DependenciesDir}", fullDependenciesDir);
        }
    }

    /// <summary>
    /// Tracks DLLs in the specified directory and adds them to the dependency list.
    /// </summary>
    /// <param name="directory">The directory to scan for DLLs.</param>
    /// <param name="isPlugin">Indicates if the DLL is a plugin or dependency.</param>
    private void TrackDllsInDirectory(string directory, bool isPlugin)
    {
        foreach (var dllPath in Directory.GetFiles(directory, "*.dll"))
        {
            var dllName = Path.GetFileName(dllPath);
            _allDependencies.Add(new DependencyModel(dllName, dllPath, isPlugin));
        }
    }

    /// <summary>
    /// Determines if a file is a native library.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is a native library; otherwise, false.</returns>
    public bool IsNativeLibrary(string filePath)
    {
        if (!File.Exists(filePath)) return false;

        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs);

            if (reader.ReadUInt16() != 0x5A4D) return false; // 'MZ' header check
            fs.Seek(0x3C, SeekOrigin.Begin);
            var peHeaderOffset = reader.ReadInt32();
            fs.Seek(peHeaderOffset, SeekOrigin.Begin);

            if (reader.ReadUInt32() != 0x4550) return false; // 'PE\0\0' header check

            var machine = reader.ReadUInt16();
            return machine != 0x14C && machine != 0x8664; // Not x86 or x64
        }
        catch
        {
            return false;
        }
    }
}