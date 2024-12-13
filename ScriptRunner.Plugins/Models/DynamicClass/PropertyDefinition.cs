namespace ScriptRunner.Plugins.Models.DynamicClass;

/// <summary>
/// Represents the definition of a property, including its name, type, accessors, and visibility.
/// </summary>
public class PropertyDefinition
{
    /// <summary>
    /// Gets or sets the name of the property.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the property.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the accessor type for the property.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>init</c> if no accessor type is specified.
    /// </remarks>
    public string AccessorType { get; set; } = "init";

    /// <summary>
    /// Gets or sets the visibility of the property accessor (e.g., <c>public</c>, <c>private</c>, etc.).
    /// </summary>
    public string AccessorVisibility { get; set; } = string.Empty;
}
