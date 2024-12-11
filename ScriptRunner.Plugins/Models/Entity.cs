using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a generic entity in the graph, consisting of a name and associated attributes.
/// </summary>
public class Entity
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity" /> class.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="attributes">The attributes associated with the entity.</param>
    public Entity(string name, Dictionary<string, object> attributes)
    {
        Name = name;
        Attributes = attributes;
    }

    /// <summary>
    ///     Gets or sets the name of the entity.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the attributes associated with the entity.
    ///     Attributes contain additional information like fields, properties, labels, etc.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; }
}