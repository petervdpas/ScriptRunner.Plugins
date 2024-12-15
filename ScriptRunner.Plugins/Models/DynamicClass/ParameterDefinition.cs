namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
///     Represents the definition of a parameter, including its name and type.
/// </summary>
public class ParameterDefinition
{
    /// <summary>
    ///     Gets or sets the name of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the type of the parameter.
    /// </summary>
    public string Type { get; set; } = string.Empty;
}