using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility to dynamically load dependencies from a specified directory.
/// </summary>
/// <remarks>
/// Dependencies are only loaded for plugins that pass pre-validation.
/// </remarks>
public static class DependencyLoader
{
    private static readonly ConcurrentDictionary<string, bool> LibraryCache = new();
    private static readonly ConcurrentBag<string> SkipLibraryChecks = [];

    /// <summary>
    /// Sets libraries to be skipped during dependency validation.
    /// </summary>
    /// <param name="skipLibraryChecks">Array of library names to skip.</param>
    public static void SetSkipLibraries(string[]? skipLibraryChecks)
    {
        if (skipLibraryChecks == null)
            return;

        foreach (var lib in skipLibraryChecks)
        {
            if (!SkipLibraryChecks.Contains(lib))
                SkipLibraryChecks.Add(lib);
        }
    }
    
    /// <summary>
    /// Loads all DLLs from the specified directory into the application domain.
    /// </summary>
    /// <param name="dependenciesDirectory">The path to the directory containing dependency DLLs.</param>
    /// <param name="loadedDependencies">A thread-safe cache to track loaded dependencies.</param>
    /// <param name="logger">The optional logger instance for logging information.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="dependenciesDirectory"/> is null or empty.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the specified <paramref name="dependenciesDirectory"/> does not exist.
    /// </exception>
    public static void LoadDependencies(
        string dependenciesDirectory, 
        ConcurrentDictionary<string, bool> loadedDependencies, 
        ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(dependenciesDirectory))
            throw new ArgumentNullException(nameof(dependenciesDirectory), "Dependencies directory cannot be null or empty.");

        if (!Directory.Exists(dependenciesDirectory))
            throw new DirectoryNotFoundException($"Dependencies directory not found: {dependenciesDirectory}");

        var dllFiles = Directory.GetFiles(dependenciesDirectory, "*.dll");
        var exceptions = new List<Exception>();

        foreach (var dll in dllFiles)
        {
            if (loadedDependencies.ContainsKey(dll))
            {
                logger?.LogDebug("Skipping already loaded dependency: {DependencyPath}", dll);
                continue;
            }

            if (ShouldSkipLibrary(dll))
            {
                logger?.LogDebug("Skipping library: {DependencyPath}", dll);
                continue;
            }
            
            try
            {
                Assembly.LoadFrom(dll);
                loadedDependencies.TryAdd(dll, true);
                logger?.LogDebug("Successfully loaded dependency: {DependencyPath}", dll);
            }
            catch (BadImageFormatException ex)
            {
                logger?.LogWarning("Native library skipped: {DependencyPath}. Error: {Message}", dll, ex.Message);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load dependency: {DependencyPath}", dll);
                exceptions.Add(new Exception($"Error loading dependency {dll}: {ex.Message}", ex));
            }
        }
        
        if (exceptions.Count != 0)
            throw new AggregateException("One or more dependencies failed to load.", exceptions);
    }
    
    /// <summary>
    /// Determines if a library should be skipped based on its name or type.
    /// </summary>
    /// <param name="filePath">The path to the library file.</param>
    /// <returns>True if the library should be skipped, false otherwise.</returns>
    private static bool ShouldSkipLibrary(string filePath)
    {
        // Check if explicitly skipped by the user
        return IsUserSkippedLibrary(filePath) ||
               // Check if the library is a native library
               LibraryCache.GetOrAdd(filePath, InspectFileForNativeLibrary);
    }

    /// <summary>
    /// Checks if the library is in the user-specified skip list.
    /// </summary>
    /// <param name="filePath">The path to the library file.</param>
    /// <returns>True if the library is in the skip list, false otherwise.</returns>
    private static bool IsUserSkippedLibrary(string filePath)
    {
        var libraryName = Path.GetFileName(filePath);
        return SkipLibraryChecks.Contains(libraryName, StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Inspects a file to determine if it is a native library.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is a native library, false otherwise.</returns>
    private static bool InspectFileForNativeLibrary(string filePath)
    {
        // Pre-filter known native library names
        if (Path.GetFileName(filePath).Contains("sqlite", StringComparison.OrdinalIgnoreCase))
            return true;

        // Ensure the file exists
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

            // If the machine type is not for .NET assemblies (e.g., x86 or x64), consider it native
            // IMAGE_FILE_MACHINE_I386 (x86) and IMAGE_FILE_MACHINE_AMD64 (x64)
            return machine != 0x14C && machine != 0x8664;
        }
        catch
        {
            // If any exception occurs, assume it's not a native library
            return false;
        }
    }
}

