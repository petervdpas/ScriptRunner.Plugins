using System.Collections.Generic;

namespace ScriptRunner.Plugins.Models;

/// <summary>
///     Represents a relationship between two entities in the graph.
/// </summary>
public class Relationship
{
    /// <summary>
    ///     Gets or sets the name of the entity from which the relationship originates.
    /// </summary>
    public string FromEntity { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the entity to which the relationship points.
    /// </summary>
    public string ToEntity { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the key associated with the relationship.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the metadata associated with the relationship.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}