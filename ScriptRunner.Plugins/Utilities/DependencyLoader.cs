using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility to dynamically load dependencies from a specified directory.
/// </summary>
public static class DependencyLoader
{
    private static readonly ConcurrentDictionary<string, Assembly> LoadedAssemblies = new();
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
    /// Loads all DLLs from the specified directory into the application domain.
    /// </summary>
    /// <param name="dependenciesDirectory">The path to the directory containing dependency DLLs.</param>
    /// <param name="logger">The optional logger instance for logging information.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="dependenciesDirectory"/> is null or empty.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the specified <paramref name="dependenciesDirectory"/> does not exist.
    /// </exception>
    public static void LoadDependencies(string dependenciesDirectory, ILogger? logger = null)
    {
        if (string.IsNullOrWhiteSpace(dependenciesDirectory))
            throw new ArgumentNullException(nameof(dependenciesDirectory), "Dependencies directory cannot be null or empty.");

        if (!Directory.Exists(dependenciesDirectory))
            throw new DirectoryNotFoundException($"Dependencies directory not found: {dependenciesDirectory}");

        var dllFiles = Directory.GetFiles(dependenciesDirectory, "*.dll");

        foreach (var dll in dllFiles)
        {
            var assemblyName = AssemblyName.GetAssemblyName(dll).FullName;
            if (LoadedAssemblies.ContainsKey(assemblyName))
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
                var assembly = Assembly.LoadFrom(dll);
                LoadedAssemblies[assemblyName] = assembly;
                logger?.LogDebug("Successfully loaded dependency: {DependencyPath}", dll);
            }
            catch (BadImageFormatException ex)
            {
                logger?.LogWarning("Native library skipped: {DependencyPath}. Error: {Message}", dll, ex.Message);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load dependency: {DependencyPath}", dll);
            }
        }
    }

    /// <summary>
    /// Determines if a library should be skipped based on its name or type.
    /// </summary>
    /// <param name="filePath">The path to the library file.</param>
    /// <returns>True if the library should be skipped, false otherwise.</returns>
    private static bool ShouldSkipLibrary(string filePath)
    {
        // Check if explicitly skipped by the user
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
}
