using System.Reflection;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Utility to dynamically load dependencies from a specified directory.
/// </summary>
/// <remarks>
/// Dependencies are only loaded for plugins that pass pre-validation.
/// </remarks>
public static class DependencyLoader
{
    /// <summary>
    /// Loads all DLLs from the specified directory into the application domain.
    /// </summary>
    /// <param name="dependenciesDirectory">The path to the directory containing dependency DLLs.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="dependenciesDirectory"/> is null or empty.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if the specified <paramref name="dependenciesDirectory"/> does not exist.
    /// </exception>
    public static void LoadDependencies(string dependenciesDirectory)
    {
        if (string.IsNullOrWhiteSpace(dependenciesDirectory))
            throw new ArgumentNullException(nameof(dependenciesDirectory), "Dependencies directory cannot be null or empty.");

        if (!Directory.Exists(dependenciesDirectory))
            throw new DirectoryNotFoundException($"Dependencies directory not found: {dependenciesDirectory}");

        var dllFiles = Directory.GetFiles(dependenciesDirectory, "*.dll");

        foreach (var dll in dllFiles)
        {
            try
            {
                Assembly.LoadFrom(dll);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load dependency {dll}: {ex.Message}");
            }
        }
    }
}