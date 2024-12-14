using System.Collections.Generic;
using System.Linq;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for Dictionary operations where the values are strings.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Finds the longest value (by string length) in a dictionary where the values are strings.
    /// </summary>
    /// <param name="dictionary">The dictionary to search.</param>
    /// <returns>The longest string value in the dictionary, or null if the dictionary is empty.</returns>
    public static string? GetLongestValue(this Dictionary<string, string>? dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
            return null;

        return dictionary.Values.OrderByDescending(v => v.Length).FirstOrDefault();
    }

    /// <summary>
    ///     Finds the length of the longest value (by string length) in a dictionary where the values are strings.
    /// </summary>
    /// <param name="dictionary">The dictionary to search.</param>
    /// <returns>The length of the longest string value in the dictionary, or 0 if the dictionary is empty.</returns>
    public static int GetLongestValueLength(this Dictionary<string, string>? dictionary)
    {
        if (dictionary == null || dictionary.Count == 0)
            return 0;

        return dictionary.Values.Max(v => v.Length);
    }
}