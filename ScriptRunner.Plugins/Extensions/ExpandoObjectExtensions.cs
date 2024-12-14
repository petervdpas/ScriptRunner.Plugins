using System.Dynamic;
using System.Text;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ExpandoObject"/> operations.
/// </summary>
public static class ExpandoObjectExtensions
{
    /// <summary>
    /// Dumps the <see cref="ExpandoObject"/> content to a readable string.
    /// </summary>
    /// <param name="expando">The <see cref="ExpandoObject"/> to dump.</param>
    /// <param name="label">An optional label to include before the output.</param>
    /// <returns>A formatted string representation of the <see cref="ExpandoObject"/>.</returns>
    public static string Dump(this ExpandoObject expando, string? label = null)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(label))
        {
            sb.AppendLine(label);
            sb.AppendLine(new string('=', label.Length)); // Underline the label
        }

        sb.AppendLine(FormatExpandoObject(expando));
        return sb.ToString();
    }

    /// <summary>
    /// Formats an <see cref="ExpandoObject"/> into a readable string representation.
    /// </summary>
    /// <param name="expando">The <see cref="ExpandoObject"/> to format.</param>
    /// <returns>A string representation of the <see cref="ExpandoObject"/>.</returns>
    private static string FormatExpandoObject(ExpandoObject expando)
    {
        var sb = new StringBuilder();

        foreach (var kvp in expando)
        {
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        }

        return sb.ToString();
    }
}