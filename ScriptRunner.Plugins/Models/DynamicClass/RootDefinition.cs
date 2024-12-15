using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
///     Represents the root structure of a code definition, including namespace, using directives, and class definitions.
/// </summary>
public class RootDefinition
{
    /// <summary>
    ///     Gets or sets the namespace for the code structure.
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the list of using directives for the code.
    /// </summary>
    /// <remarks>
    ///     Each entry in the list represents a namespace that is imported for use in the generated code.
    /// </remarks>
    public List<string> Usings { get; set; } = [];

    /// <summary>
    ///     Gets or sets the list of class definitions contained within the root structure.
    /// </summary>
    /// <remarks>
    ///     Each class definition includes details about its properties, methods, constructors, and implemented interfaces.
    /// </remarks>
    public List<ClassDefinition> Classes { get; set; } = [];
}