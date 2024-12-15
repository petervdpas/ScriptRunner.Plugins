using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
///     Represents the definition of a method, including its name, return type, parameters, and body.
/// </summary>
public class MethodDefinition
{
    /// <summary>
    ///     Gets or sets the name of the method.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the return type of the method.
    /// </summary>
    /// <remarks>
    ///     Defaults to <c>void</c> if no return type is specified.
    /// </remarks>
    public string ReturnType { get; set; } = "void";

    /// <summary>
    ///     Gets or sets the list of parameters for the method.
    /// </summary>
    /// <remarks>
    ///     Each parameter is represented by a <see cref="ParameterDefinition" /> object.
    /// </remarks>
    public List<ParameterDefinition> Parameters { get; set; } = [];

    /// <summary>
    ///     Gets or sets the lines of code that make up the body of the method.
    /// </summary>
    /// <remarks>
    ///     Each line is represented as a string in the list.
    /// </remarks>
    public List<string> BodyLines { get; set; } = [];
}