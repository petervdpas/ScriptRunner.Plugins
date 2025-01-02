using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods for working with <see cref="ExpandoObject"/> instances.
/// </summary>
/// <remarks>
/// The <see cref="ExpandoObjectHelper"/> class includes methods for displaying, manipulating, 
/// and interacting with <see cref="ExpandoObject"/> instances. These methods are designed to 
/// simplify common tasks such as inspecting key-value pairs or validating the contents of 
/// dynamic objects.
/// </remarks>
public static class ExpandoObjectHelper
{
    /// <summary>
    /// Displays all key-value pairs in an <see cref="ExpandoObject"/> in a user-friendly format.
    /// </summary>
    /// <param name="expando">The <see cref="ExpandoObject"/> to display. Can be <c>null</c>.</param>
    /// <remarks>
    /// This method handles the following scenarios:
    /// <list type="bullet">
    /// <item><description>If the <paramref name="expando"/> is <c>null</c>, it outputs a null message.</description></item>
    /// <item><description>If the <paramref name="expando"/> is empty, it outputs an empty message.</description></item>
    /// <item><description>If the <paramref name="expando"/> contains values, it lists all key-value pairs.</description></item>
    /// </list>
    /// </remarks>
    public static void DisplayValues(ExpandoObject? expando)
    {
        switch (expando)
        {
            case null:
                Console.WriteLine("The provided ExpandoObject is null.");
                return;
            // Safely cast to IDictionary<string, object>
            case IDictionary<string, object> { Count: 0 }:
                Console.WriteLine("The ExpandoObject is empty.");
                return;
            case IDictionary<string, object> dictionary:
            {
                Console.WriteLine("ExpandoObject values:");
                foreach (var kvp in dictionary)
                {
                    Console.WriteLine($"- {kvp.Key}: {kvp.Value}");
                }

                break;
            }
        }
    }
}