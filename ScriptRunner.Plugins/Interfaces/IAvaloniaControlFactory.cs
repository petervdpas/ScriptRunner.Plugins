using System;
using System.Collections.Generic;
using System.Dynamic;
using Avalonia.Controls;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface for generating Avalonia controls.
/// </summary>
public interface IAvaloniaControlFactory
{
    /// <summary>
    /// Generates Avalonia controls based on an ExpandoObject's properties.
    /// </summary>
    /// <param name="expando">The ExpandoObject to generate controls for.</param>
    /// <returns>A collection of Avalonia controls.</returns>
    IEnumerable<Control> GenerateControls(ExpandoObject expando);

    /// <summary>
    /// Generates Avalonia controls for a dynamic type and record item.
    /// </summary>
    /// <param name="dynamicType">The dynamic type defining the properties.</param>
    /// <param name="recordItem">The record item to bind data to.</param>
    /// <returns>A collection of Avalonia controls.</returns>
    IEnumerable<Control> GenerateControls(Type? dynamicType, RecordItem recordItem);
}