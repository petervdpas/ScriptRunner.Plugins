using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for <see cref="DataTable" /> operations.
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    ///     Dumps the <see cref="DataTable" /> content to a readable string.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to dump.</param>
    /// <param name="label">An optional label to include before the output.</param>
    /// <returns>A formatted string representation of the <see cref="DataTable" />.</returns>
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
    ///     Formats a <see cref="DataTable" /> into a readable string format.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to format.</param>
    /// <returns>A formatted string representation of the <see cref="DataTable" />.</returns>
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
            output.Write($"| {table.Columns[i].ColumnName.PadRight(columnWidths[i])} ");
        output.WriteLine("|");
        output.WriteLine(separatorLine);

        // Write each row
        foreach (DataRow row in table.Rows)
        {
            for (var i = 0; i < table.Columns.Count; i++)
                output.Write($"| {row[i].ToString()?.PadRight(columnWidths[i])} ");
            output.WriteLine("|");
        }

        output.WriteLine(separatorLine);

        return output.ToString();
    }

    /// <summary>
    ///     Creates a horizontal separator line for the <see cref="DataTable" />.
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

    /// <summary>
    ///     Converts a collection of objects into a <see cref="DataTable" />.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection.</typeparam>
    /// <param name="data">The collection of objects to convert.</param>
    /// <returns>A <see cref="DataTable" /> representation of the provided collection.</returns>
    public static DataTable ToDataTable<T>(this IEnumerable<T> data)
    {
        var dataTable = new DataTable();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)).ToArray();
            dataTable.Rows.Add(values);
        }

        return dataTable;
    }

    /// <summary>
    ///     Filters the rows of a <see cref="DataTable" /> based on a predicate.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to filter.</param>
    /// <param name="predicate">A function to test each row for a condition.</param>
    /// <returns>A new <see cref="DataTable" /> containing rows that satisfy the predicate.</returns>
    public static DataTable Filter(this DataTable table, Func<DataRow, bool> predicate)
    {
        var filteredTable = table.Clone(); // Copy structure, no data
        foreach (DataRow row in table.Rows)
            if (predicate(row))
                filteredTable.ImportRow(row);
        return filteredTable;
    }

    /// <summary>
    ///     Sorts the rows of a <see cref="DataTable" /> by a specified column.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to sort.</param>
    /// <param name="columnName">The column name to sort by.</param>
    /// <param name="ascending">Whether to sort in ascending order. Defaults to true.</param>
    /// <returns>A new <see cref="DataTable" /> with rows sorted by the specified column.</returns>
    public static DataTable Sort(this DataTable table, string columnName, bool ascending = true)
    {
        var sortedView = table.DefaultView;
        sortedView.Sort = $"{columnName} {(ascending ? "ASC" : "DESC")}";
        return sortedView.ToTable();
    }

    /// <summary>
    ///     Adds a row to a <see cref="DataTable" /> with specified column-value pairs.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to add a row to.</param>
    /// <param name="values">A dictionary of column names and their corresponding values.</param>
    public static void AddRow(this DataTable table, Dictionary<string, object> values)
    {
        var newRow = table.NewRow();
        foreach (var kvp in values.Where(kvp => table.Columns.Contains(kvp.Key)))
            newRow[kvp.Key] = kvp.Value ?? DBNull.Value;
        table.Rows.Add(newRow);
    }

    /// <summary>
    ///     Exports the <see cref="DataTable" /> to a CSV string.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to export.</param>
    /// <param name="includeHeaders">Whether to include column headers in the output. Defaults to true.</param>
    /// <returns>A CSV-formatted string representing the <see cref="DataTable" />.</returns>
    public static string ToCsv(this DataTable table, bool includeHeaders = true)
    {
        var sb = new StringBuilder();

        if (includeHeaders) sb.AppendLine(string.Join(",", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

        foreach (DataRow row in table.Rows)
            sb.AppendLine(string.Join(",", row.ItemArray.Select(field => field?.ToString() ?? string.Empty)));

        return sb.ToString();
    }

    /// <summary>
    ///     Determines whether the <see cref="DataTable" /> is empty (has no rows).
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to check.</param>
    /// <returns>True if the table is empty; otherwise, false.</returns>
    public static bool IsEmpty(this DataTable table)
    {
        return table.Rows.Count == 0;
    }

    /// <summary>
    ///     Gets distinct rows from a <see cref="DataTable" /> based on specified columns.
    /// </summary>
    /// <param name="table">The <see cref="DataTable" /> to filter.</param>
    /// <param name="columnNames">The columns to use for determining distinct rows.</param>
    /// <returns>A new <see cref="DataTable" /> with distinct rows.</returns>
    public static DataTable GetDistinctRows(this DataTable table, params string[] columnNames)
    {
        var view = new DataView(table);
        return view.ToTable(true, columnNames);
    }
}