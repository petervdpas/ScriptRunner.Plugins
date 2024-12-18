using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Service for tracking plugins and their dependencies.
/// </summary>
public class PluginTracker : IPluginTracker
{
    private readonly List<DependencyModel> _allDependencies = [];
    private static readonly ConcurrentDictionary<string, PluginLoadContext> PluginContexts = new();
    private static readonly HashSet<string> SkipLibraryChecks = new(StringComparer.OrdinalIgnoreCase);

    private readonly string _pluginRootDirectory;
    private readonly string _dependenciesDirectory;
    
    private readonly ILogger<PluginTracker> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginTracker"/> class.
    /// </summary>
    /// <param name="logger">The logger for tracking operations.</param>
    /// <param name="pluginRootDirectory">The root directory containing plugin subdirectories.</param>
    /// <param name="dependenciesDirectory">The name of the directory where dependencies are stored.</param>
    public PluginTracker(ILogger<PluginTracker> logger, string pluginRootDirectory, string dependenciesDirectory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pluginRootDirectory = pluginRootDirectory;
        _dependenciesDirectory = dependenciesDirectory;
    }

    /// <summary>
    ///     Sets libraries to be skipped during dependency validation.
    /// </summary>
    /// <param name="skipLibraryChecks">Array of library names to skip.</param>
    public void SetSkipLibraries(IEnumerable<string>? skipLibraryChecks)
    {
        if (skipLibraryChecks == null) return;

        foreach (var lib in skipLibraryChecks) SkipLibraryChecks.Add(lib);
    }
    
    /// <summary>
    /// Discovers plugins in the specified root directory, extracts their metadata,
    /// and tracks their related dependencies.
    /// </summary>
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
                var pluginName = PluginMetadataChecker.GetPluginNameIfExists(pluginDll);
                if (!string.IsNullOrEmpty(pluginName))
                {
                    var dllName = Path.GetFileName(pluginDll);
                    _allDependencies.Add(new DependencyModel(dllName, pluginDll, true, pluginName));
                    TrackDependencies(pluginDir, _dependenciesDirectory, pluginName);
                }
                else
                {
                    _logger.LogWarning($"No plugin metadata found in DLL: {pluginDll}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process plugin directory '{pluginDir}': {ex.Message}");
            }
        }
    }

    /// <inheritdoc/>
    /// <summary>
    /// Returns all tracked main plugin DLLs.
    /// </summary>
    /// <returns>A list of <see cref="DependencyModel"/> representing main plugin DLLs.</returns>
    public List<DependencyModel> GetTrackedPlugins()
    {
        return _allDependencies
            .Where(d => d.IsPlugin())
            .ToList();
    }
    
    /// <summary>
    /// Returns all tracked dependency DLLs (excluding main plugins).
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
    /// Loads a dependency DLL into an isolated plugin context and tracks it.
    /// </summary>
    /// <param name="pluginName">The name of the plugin owning the dependency.</param>
    /// <param name="directory">The directory where the DLL is located.</param>
    /// <param name="dllName">The name of the DLL to load.</param>
    public void LoadDependency(string pluginName, string directory, string dllName)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"Directory not found: {directory}");

        var fullPath = Path.Combine(directory, dllName);
        if (!File.Exists(fullPath))
        {
            _logger.LogError("> DLL not found: {DllName}", dllName);
            return;
        }

        // Skip libraries flagged for skipping
        if (ShouldSkipLibrary(dllName))
        {
            _logger.LogDebug("Skipping library: {DllName}", dllName);
            return;
        }

        try
        {
            // Use or create a new PluginLoadContext for the plugin
            var loadContext = PluginContexts.GetOrAdd(pluginName, name => new PluginLoadContext(name));

            // Load the assembly into the isolated context
            loadContext.LoadFromAssemblyPath(fullPath);

            // Track the loaded dependency
            _allDependencies.Add(new DependencyModel(dllName, fullPath, false, pluginName));
            _logger.LogInformation("Successfully loaded DLL: {DllName} into context: {PluginName}", dllName, pluginName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load DLL: {DllPath}", fullPath);
        }
    }

    /// <summary>
    /// Tracks all DLLs in a plugin directory and its host-defined dependencies directory.
    /// </summary>
    /// <param name="pluginDirectory">The root directory of the plugin.</param>
    /// <param name="dependenciesDirectory">The name of the dependency directory.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    private void TrackDependencies(string pluginDirectory, string dependenciesDirectory, string pluginName)
    {
        var fullDependenciesDir = Path.Combine(pluginDirectory, dependenciesDirectory);
        if (Directory.Exists(fullDependenciesDir))
        {
            TrackDllsInDirectory(fullDependenciesDir, pluginName, false);
        }
        else
        {
            _logger.LogWarning($"Dependencies directory not found: {fullDependenciesDir}");
        }
    }

    /// <summary>
    /// Tracks DLLs in the specified directory and adds them to the dependency list.
    /// </summary>
    /// <param name="directory">The directory to scan for DLLs.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    /// <param name="isPlugin">Indicates if the DLL is a plugin or dependency.</param>
    private void TrackDllsInDirectory(string directory, string pluginName, bool isPlugin)
    {
        foreach (var dllPath in Directory.GetFiles(directory, "*.dll"))
        {
            var dllName = Path.GetFileName(dllPath);
            _allDependencies.Add(new DependencyModel(dllName, dllPath, isPlugin, pluginName));
        }
    }
    
    /// <summary>
    /// Determines if a library should be skipped based on its name or type.
    /// </summary>
    /// <param name="fileName">The name of the library file.</param>
    /// <returns>True if the library should be skipped; otherwise, false.</returns>
    private bool ShouldSkipLibrary(string fileName)
    {
        return SkipLibraryChecks.Contains(fileName) || IsNativeLibrary(fileName);
    }

    /// <summary>
    /// Inspects a file to determine if it is a native library.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is a native library; otherwise, false.</returns>
    private static bool IsNativeLibrary(string filePath)
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

    /// <summary>
    /// Custom AssemblyLoadContext for isolating plugin dependencies.
    /// </summary>
    private class PluginLoadContext(string name) : AssemblyLoadContext(name, true)
    {
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null; // Defer to default AssemblyLoadContext
        }
    }
}