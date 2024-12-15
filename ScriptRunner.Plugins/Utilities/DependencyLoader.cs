using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility to dynamically load dependencies from a specified directory with support for shared and isolated contexts.
/// </summary>
public static class DependencyLoader
{
    private static readonly ConcurrentDictionary<string, PluginLoadContext> PluginContexts = new();
    private static readonly ConcurrentDictionary<string, Assembly> GlobalSharedAssemblies = new();
    private static readonly HashSet<string> SkipLibraryChecks = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Sets libraries to be skipped during dependency validation.
    /// </summary>
    /// <param name="skipLibraryChecks">Array of library names to skip.</param>
    public static void SetSkipLibraries(IEnumerable<string>? skipLibraryChecks)
    {
        if (skipLibraryChecks == null) return;

        foreach (var lib in skipLibraryChecks)
        {
            SkipLibraryChecks.Add(lib);
        }
    }

    /// <summary>
    /// Loads all DLLs from the specified directory into an isolated AssemblyLoadContext or a global shared context.
    /// </summary>
    /// <param name="pluginName">The unique name of the plugin.</param>
    /// <param name="dependenciesDirectory">The path to the directory containing dependency DLLs.</param>
    /// <param name="sharedDependencies">The list of DLLs to be loaded into the global shared context.</param>
    /// <param name="logger">The optional logger instance for logging information.</param>
    public static void LoadDependencies(
        string pluginName, 
        string dependenciesDirectory, 
        IEnumerable<string> sharedDependencies, 
        ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(dependenciesDirectory))
            throw new ArgumentNullException(nameof(dependenciesDirectory), "Dependencies directory cannot be null or empty.");

        if (!Directory.Exists(dependenciesDirectory))
            throw new DirectoryNotFoundException($"Dependencies directory not found: {dependenciesDirectory}");

        // Convert sharedDependencies to a HashSet for fast lookups
        var sharedDependenciesSet = new HashSet<string>(sharedDependencies, StringComparer.OrdinalIgnoreCase);

        var dllFiles = Directory.GetFiles(dependenciesDirectory, "*.dll");

        // Create or retrieve the plugin's AssemblyLoadContext
        var loadContext = PluginContexts.GetOrAdd(pluginName, name => new PluginLoadContext(name));

        foreach (var dll in dllFiles)
        {
            var libraryName = Path.GetFileName(dll);

            if (ShouldSkipLibrary(dll))
            {
                logger?.LogDebug("Skipping library: {DependencyPath}", dll);
                continue;
            }

            if (sharedDependenciesSet.Contains(libraryName))
            {
                // Load into the global shared context
                LoadSharedDependency(dll, libraryName, logger);
            }
            else
            {
                // Load into the plugin's isolated context
                LoadIsolatedDependency(dll, loadContext, pluginName, logger);
            }
        }
    }

    /// <summary>
    /// Loads a shared dependency into the global shared context.
    /// </summary>
    /// <param name="dependencyPath">The path to the dependency DLL.</param>
    /// <param name="libraryName">The name of the library.</param>
    /// <param name="logger">The optional logger instance for logging information.</param>
    private static void LoadSharedDependency(string dependencyPath, string libraryName, ILogger? logger)
    {
        if (GlobalSharedAssemblies.ContainsKey(libraryName))
        {
            logger?.LogDebug("Skipping already loaded shared dependency: {LibraryName}", libraryName);
            return;
        }

        try
        {
            var assembly = Assembly.LoadFrom(dependencyPath);
            GlobalSharedAssemblies[libraryName] = assembly;
            logger?.LogDebug("Successfully loaded shared dependency: {LibraryName}", libraryName);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load shared dependency: {LibraryName}", libraryName);
        }
    }

    /// <summary>
    /// Loads a dependency into the specified plugin's isolated AssemblyLoadContext.
    /// </summary>
    /// <param name="dependencyPath">The path to the dependency DLL.</param>
    /// <param name="loadContext">The plugin's AssemblyLoadContext.</param>
    /// <param name="pluginName">The name of the plugin.</param>
    /// <param name="logger">The optional logger instance for logging information.</param>
    private static void LoadIsolatedDependency(
        string dependencyPath, 
        PluginLoadContext loadContext, 
        string pluginName, 
        ILogger? logger)
    {
        try
        {
            loadContext.LoadFromAssemblyPath(dependencyPath);
            logger?.LogDebug("Successfully loaded dependency: {DependencyPath} into context: {PluginName}", dependencyPath, pluginName);
        }
        catch (BadImageFormatException ex)
        {
            logger?.LogWarning("Native library skipped: {DependencyPath}. Error: {Message}", dependencyPath, ex.Message);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to load dependency: {DependencyPath}", dependencyPath);
        }
    }

    /// <summary>
    /// Determines if a library should be skipped based on its name or type.
    /// </summary>
    /// <param name="filePath">The path to the library file.</param>
    /// <returns>True if the library should be skipped, false otherwise.</returns>
    private static bool ShouldSkipLibrary(string filePath)
    {
        var libraryName = Path.GetFileName(filePath);
        return SkipLibraryChecks.Contains(libraryName) || InspectFileForNativeLibrary(filePath);
    }

    /// <summary>
    /// Inspects a file to determine if it is a native library.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is a native library, false otherwise.</returns>
    private static bool InspectFileForNativeLibrary(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(fs);

            // Check for the "MZ" header
            if (reader.ReadUInt16() != 0x5A4D) // 'MZ' in little-endian
                return false;

            // Move to the PE header offset
            fs.Seek(0x3C, SeekOrigin.Begin);
            var peHeaderOffset = reader.ReadInt32();

            // Check for the "PE" signature
            fs.Seek(peHeaderOffset, SeekOrigin.Begin);
            if (reader.ReadUInt32() != 0x4550) // 'PE\0\0' in little-endian
                return false;

            // Read the Machine field to determine architecture
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
    private class PluginLoadContext : AssemblyLoadContext
    {
        public PluginLoadContext(string name) : base(name, isCollectible: true) { }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // Prevent circular loading by deferring to the default context
            return null;
        }
    }
}