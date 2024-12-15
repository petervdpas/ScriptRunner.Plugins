using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
///     Represents the definition of a class, including its name, interfaces, properties, constructors, and methods.
/// </summary>
public class ClassDefinition
{
    /// <summary>
    ///     Gets or sets the name of the class.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the list of interfaces implemented by the class.
    /// </summary>
    public List<string> Implements { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of properties defined in the class.
    /// </summary>
    public List<PropertyDefinition> Properties { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of constructors defined in the class,
    ///     each represented with its parameters and body lines.
    /// </summary>
    public List<ConstructorDefinition> Constructors { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of methods defined in the class.
    /// </summary>
    public List<MethodDefinition> Methods { get; set; } = [];
}