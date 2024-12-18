namespace ScriptRunner.Plugins.Models;

/// <summary>
/// Represents a tracked DLL with its metadata.
/// </summary>
public class DependencyModel
{
    /// <summary>
    /// The name of the DLL.
    /// </summary>
    private readonly string _dllName;

    /// <summary>
    /// The full file path to the DLL.
    /// </summary>
    private readonly string _fullPath;

    /// <summary>
    /// Indicates whether the DLL is the main plugin or a dependency.
    /// </summary>
    private readonly bool _isPlugin;

    /// <summary>
    /// The name of the plugin the DLL belongs to.
    /// </summary>
    private readonly string _pluginName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyModel"/> class.
    /// </summary>
    /// <param name="dllName">The name of the DLL.</param>
    /// <param name="fullPath">The full file path of the DLL.</param>
    /// <param name="isPlugin">Whether the DLL is the main plugin.</param>
    /// <param name="pluginName">The plugin name associated with this DLL.</param>
    public DependencyModel(string dllName, string fullPath, bool isPlugin, string pluginName)
    {
        _dllName = dllName;
        _fullPath = fullPath;
        _isPlugin = isPlugin;
        _pluginName = pluginName;
    }
    
    /// <summary>
    /// Checks whether this dependency is the main plugin DLL.
    /// </summary>
    /// <returns>True if the dependency is a main plugin, otherwise false.</returns>
    public bool IsPlugin()
    {
        return _isPlugin;
    }

    /// <summary>
    /// Retrieves the metadata of the dependency as a tuple.
    /// </summary>
    /// <returns>A tuple containing dll name, full path, isPlugin flag, and plugin name.</returns>
    public (string DllName, string FullPath, bool IsPlugin, string PluginName) GetTuple()
    {
        return (_dllName, _fullPath, _isPlugin, _pluginName);
    }
    
    /// <summary>
    /// Provides a string representation of the dependency for debugging purposes.
    /// </summary>
    /// <returns>A formatted string containing the dependency details.</returns>
    public override string ToString()
    {
        return $"[Dependency] Name: {_dllName}, Path: {_fullPath}, IsPlugin: {_isPlugin}, PluginName: {_pluginName}";
    }
}