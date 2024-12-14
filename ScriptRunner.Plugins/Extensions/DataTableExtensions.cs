using System.Data;
using System.IO;
using System.Text;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DataTable"/> operations.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// Dumps the <see cref="DataTable"/> content to a readable string.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> to dump.</param>
    /// <param name="label">An optional label to include before the output.</param>
    /// <returns>A formatted string representation of the <see cref="DataTable"/>.</returns>
    public static string Dump(this DataTable table, string? label = null)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(label))
        {
            sb.AppendLine(label);
            sb.AppendLine(new string('=', label.Length)); // Underline the label
        }

        sb.AppendLine(FormatDataTable(table));
        return sb.ToString();
    }
    
    /// <summary>
    /// Formats a <see cref="DataTable"/> into a readable string format.
    /// </summary>
    /// <param name="table">The <see cref="DataTable"/> to format.</param>
    /// <returns>A formatted string representation of the <see cref="DataTable"/>.</returns>
    private static string FormatDataTable(DataTable table)
    {
        if (table.Rows.Count == 0) return "No data found.";

        var output = new StringWriter();
        var columnWidths = new int[table.Columns.Count];

        // Calculate the maximum width for each column
        for (var i = 0; i < table.Columns.Count; i++)
        {
            columnWidths[i] = table.Columns[i].ColumnName.Length; // Start with the header length

            foreach (DataRow row in table.Rows)
            {
                var length = row[i].ToString()?.Length ?? 0;
                if (length > columnWidths[i]) columnWidths[i] = length;
            }
        }

        // Create a horizontal separator line
        var separatorLine = CreateSeparatorLine(columnWidths);

        // Write the column headers
        output.WriteLine(separatorLine);
        for (var i = 0; i < table.Columns.Count; i++)
        {
            output.Write($"| {table.Columns[i].ColumnName.PadRight(columnWidths[i])} ");
        }
        output.WriteLine("|");
        output.WriteLine(separatorLine);

        // Write each row
        foreach (DataRow row in table.Rows)
        {
            for (var i = 0; i < table.Columns.Count; i++)
            {
                output.Write($"| {row[i].ToString()?.PadRight(columnWidths[i])} ");
            }
            output.WriteLine("|");
        }

        output.WriteLine(separatorLine);

        return output.ToString();
    }

    /// <summary>
    /// Creates a horizontal separator line for the <see cref="DataTable"/>.
    /// </summary>
    /// <param name="columnWidths">The widths of the columns in the table.</param>
    /// <returns>A formatted separator line string.</returns>
    private static string CreateSeparatorLine(int[] columnWidths)
    {
        var separator = new StringBuilder();

        foreach (var width in columnWidths)
        {
            separator.Append('+');
            separator.Append(new string('-', width + 2)); // Add padding
        }

        separator.Append('+');
        return separator.ToString();
    }
}