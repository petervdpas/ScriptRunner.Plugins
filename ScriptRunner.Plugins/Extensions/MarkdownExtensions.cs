using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for working with Markdown files.
/// </summary>
public static class MarkdownExtensions
{
    /// <summary>
    ///     Parses metadata from a Markdown file.
    /// </summary>
    /// <param name="markdownFilePath">The path to the Markdown file.</param>
    /// <returns>A dictionary containing the metadata key-value pairs.</returns>
    public static IDictionary<string, string> ParseMetadata(this string markdownFilePath)
    {
        if (!File.Exists(markdownFilePath))
            throw new FileNotFoundException("The specified markdown file does not exist.", markdownFilePath);

        var lines = File.ReadLines(markdownFilePath).ToList(); // Materialize to prevent multiple enumeration
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!lines.FirstOrDefault()?.TrimStart().StartsWith("---") ?? true) return metadata;

        foreach (var line in lines.Skip(1)) // Skip first `---`
        {
            if (line.Trim().StartsWith("---")) break; // End of metadata

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex == -1) continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            metadata[key] = value.Trim('"');
        }

        return metadata;
    }
}