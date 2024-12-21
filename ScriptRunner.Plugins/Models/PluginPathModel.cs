namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a tracked plugin DLL.
/// </summary>
public class PluginPathModel
{
    /// <summary>
    ///     The name of the DLL.
    /// </summary>
    private readonly string _dllName;

    /// <summary>
    ///     The full file path to the DLL.
    /// </summary>
    private readonly string _fullPath;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluginPathModel" /> class.
    /// </summary>
    /// <param name="dllName">The name of the DLL.</param>
    /// <param name="fullPath">The full file path of the DLL.</param>
    public PluginPathModel(string dllName, string fullPath)
    {
        _dllName = dllName;
        _fullPath = fullPath;
    }

    /// <summary>
    ///     Retrieves the metadata of the dependency as a tuple.
    /// </summary>
    /// <returns>A tuple containing dll name and full path.</returns>
    public (string DllName, string FullPath) GetTuple()
    {
        return (_dllName, _fullPath);
    }

    /// <summary>
    ///     Provides a string representation of the dependency for debugging purposes.
    /// </summary>
    /// <returns>A formatted string containing the dependency details.</returns>
    public override string ToString()
    {
        return $"[Plugin] Name: {_dllName}, Path: {_fullPath}";
    }
}