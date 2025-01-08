using System;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Defines an interface for dynamically building classes with customizable properties at runtime.
/// </summary>
public interface IDynamicClassBuilder
{
    /// <summary>
    ///     Adds a new property definition to the dynamic class being constructed.
    /// </summary>
    /// <param name="dynamicProperty">
    ///     The metadata for the property to be added, including its name, type, and additional attributes.
    /// </param>
    void AddProperty(DynamicPropertyMetadata dynamicProperty);

    /// <summary>
    ///     Builds and returns the dynamically created class type based on the specified properties.
    /// </summary>
    /// <returns>The <see cref="Type" /> representing the dynamically generated class.</returns>
    Type Build();
}