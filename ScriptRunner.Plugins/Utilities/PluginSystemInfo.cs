﻿using System.Reflection;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides information about the current plugin system, including its version.
/// </summary>
public static class PluginSystemInfo
{
    /// <summary>
    /// Gets the current plugin system version from the executing assembly's metadata.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the version of the plugin system.
    /// If the version is not available, returns "Unknown".
    /// </value>
    /// <remarks>
    /// The plugin system version is derived from the <c>AssemblyInformationalVersionAttribute</c>
    /// of the executing assembly. Ensure the <c>Version</c> property is correctly set in the project file
    /// (e.g., <c>&lt;Version&gt;1.0.0&lt;/Version&gt;</c> in the <c>.csproj</c>).
    /// </remarks>
    public static string CurrentPluginSystemVersion =>
        NormalizeVersion(
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "Unknown");

    /// <summary>
    /// Normalizes a version string by removing build metadata and retaining only major.minor.patch.
    /// </summary>
    /// <param name="version">The version string to normalize.</param>
    /// <returns>The normalized version string.</returns>
    private static string NormalizeVersion(string version)
    {
        var parts = version.Split('+');
        return parts[0]; // Keep only the major.minor.patch part or unknown
    }
}