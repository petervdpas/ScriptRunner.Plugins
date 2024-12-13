using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
/// Represents the definition of a class constructor, including its parameters and body lines.
/// </summary>
public class ConstructorDefinition
{
    /// <summary>
    /// Gets or sets the list of parameters for the constructor.
    /// </summary>
    public List<ParameterDefinition> Parameters { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of lines representing the body of the constructor.
    /// </summary>
    public List<string> BodyLines { get; set; } = [];
}