using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Defines methods for generating SQL queries and mapping <see cref="DataRow" /> values to SQL parameters,
///     specifically for operations on dynamically defined types and tables.
///     Implements <see cref="ISqlGenerator" />.
/// </summary>
public class SqlGenerator : ISqlGenerator
{
    private Type? _dynamicType;
    private PropertyInfo? _idProperty;
    private List<PropertyInfo>? _properties;
    private string? _tableName;

    /// <summary>
    ///     Sets the dynamic type on which SQL operations will be based.
    /// </summary>
    /// <param name="dynamicType">The dynamic class type representing the schema of the table.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown if the specified <paramref name="dynamicType" /> does not contain a property named "Id" or "ID".
    /// </exception>
    /// <remarks>
    ///     This method should be called before generating SQL queries to ensure that the generated queries
    ///     accurately reflect the structure of the specified type, including its properties and ID field.
    /// </remarks>
    public void SetType(Type dynamicType)
    {
        _dynamicType = dynamicType;

        // Locate the ID property
        _idProperty = _dynamicType.GetProperties()
                          .FirstOrDefault(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase))
                      ?? throw new ArgumentException("The dynamic class must have an 'Id' property.");

        // Gather all other properties
        _properties = _dynamicType.GetProperties().Where(p => p != _idProperty).ToList();
    }

    /// <summary>
    ///     Sets the name of the database table for SQL query generation.
    /// </summary>
    /// <param name="tableName">The name of the database table as a string.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="tableName" /> is null or empty.</exception>
    /// <remarks>
    ///     This method should be called before generating any SQL queries, as the table name is required
    ///     for generating valid SQL statements. The table name should match the exact name of the table in the database.
    /// </remarks>
    public void SetTableName(string tableName)
    {
        _tableName = !string.IsNullOrWhiteSpace(tableName)
            ? tableName
            : throw new ArgumentException("Table name cannot be null or empty.");
    }

    /// <summary>
    ///     Generates an SQL SELECT query string that retrieves columns from the specified table.
    /// </summary>
    /// <param name="filterById">
    ///     If <c>true</c>, the generated query will include a WHERE clause to filter results by the ID column,
    ///     allowing for retrieval of a single row based on the primary key.
    /// </param>
    /// <returns>
    ///     A SQL SELECT query string that selects all columns from the table. If <paramref name="filterById" />
    ///     is <c>true</c>, the query will contain a WHERE clause that filters rows by the ID column.
    /// </returns>
    /// <remarks>
    ///     The SELECT query string is generated dynamically based on the properties of the dynamic type specified.
    /// </remarks>
    public string GenerateSelectQuery(bool filterById = false)
    {
        EnsureReady();

        var columns = string.Join(", ", _properties!.Select(p => p.Name));
        var query = $"SELECT {_idProperty!.Name}, {columns} FROM {_tableName}";

        if (filterById) query += $" WHERE {_idProperty!.Name} = @{_idProperty!.Name}";

        return query;
    }

    /// <summary>
    ///     Generates an SQL INSERT query string using the properties of the dynamic type to determine the columns.
    /// </summary>
    /// <returns>
    ///     An SQL INSERT query string that inserts values into the table columns, based on the properties of the dynamic type.
    /// </returns>
    /// <remarks>
    ///     The generated INSERT query will use parameterized placeholders (e.g., <c>@PropertyName</c>) for each column,
    ///     allowing for safe parameter binding and preventing SQL injection.
    /// </remarks>
    public string GenerateInsertQuery()
    {
        EnsureReady();

        var columns = string.Join(", ", _properties!.Select(p => p.Name));
        var parameters = string.Join(", ", _properties!.Select(p => $"@{p.Name}"));

        return $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";
    }

    /// <summary>
    ///     Generates an SQL UPDATE query string that updates columns in the table, based on the properties of the dynamic
    ///     type.
    /// </summary>
    /// <returns>
    ///     A SQL UPDATE query string that sets each column's value according to the dynamic type properties,
    ///     with a WHERE clause to filter by the ID column.
    /// </returns>
    /// <remarks>
    ///     The generated UPDATE query uses parameterized placeholders for each column and is designed to update a single row,
    ///     identified by its primary key (ID). Ensure that the ID column value is provided in the parameter dictionary
    ///     when executing the query.
    /// </remarks>
    public string GenerateUpdateQuery()
    {
        EnsureReady();

        var setClause = string.Join(", ", _properties!.Select(p => $"{p.Name} = @{p.Name}"));

        return $"UPDATE {_tableName} SET {setClause} WHERE {_idProperty!.Name} = @{_idProperty.Name}";
    }

    /// <summary>
    ///     Generates an SQL DELETE query string that deletes a row from the table, filtered by the primary key.
    /// </summary>
    /// <returns>
    ///     An SQL DELETE query string with a WHERE clause that deletes a row based on the ID value.
    /// </returns>
    /// <remarks>
    ///     The generated DELETE query is parameterized for the ID column, allowing deletion of a specific row based on its
    ///     primary key.
    ///     This query is generally used for single-row deletions.
    /// </remarks>
    public string GenerateDeleteQuery()
    {
        EnsureReady();
        return $"DELETE FROM {_tableName} WHERE {_idProperty!.Name} = @{_idProperty.Name}";
    }

    /// <summary>
    ///     Maps the values in a <see cref="DataRow" /> to a dictionary of SQL parameters,
    ///     using the dynamic type properties as the keys.
    /// </summary>
    /// <param name="row">The <see cref="DataRow" /> containing the values to be mapped.</param>
    /// <returns>
    ///     A dictionary where each key is the parameter name, prefixed with "@" for SQL compatibility,
    ///     and each value is the corresponding column value from the <see cref="DataRow" />.
    /// </returns>
    /// <remarks>
    ///     This method ensures SQL compatibility by handling any necessary formatting, such as converting
    ///     <see cref="DateTime" /> values to <c>"yyyy-MM-dd"</c> format. It includes the primary key (ID) value if it exists
    ///     in the <see cref="DataRow" />.
    ///     The generated dictionary is designed to work directly with parameterized SQL queries, ensuring safe and accurate
    ///     mapping.
    /// </remarks>
    public Dictionary<string, object> MapParameters(DataRow row)
    {
        EnsureReady();

        var parameterValues = new Dictionary<string, object>();

        // Loop through the properties and map the DataRow columns to SQL parameters
        foreach (var prop in _properties!)
        {
            var value = row[prop.Name];

            // Handle date formatting
            if (value is DateTime dateValue) value = dateValue.ToString("yyyy-MM-dd");

            parameterValues[$"@{prop.Name}"] = value;
        }

        // Map the ID column if it exists
        if (_idProperty != null && row.Table.Columns.Contains(_idProperty.Name))
            parameterValues[$"@{_idProperty.Name}"] = row[_idProperty.Name];

        return parameterValues;
    }

    /// <summary>
    ///     Checks if both dynamic type and table name have been set.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if either dynamic type or table name is not set.</exception>
    private void EnsureReady()
    {
        if (_dynamicType == null || _tableName == null || _idProperty == null || _properties == null)
            throw new InvalidOperationException(
                "Both dynamic type and table name must be set before generating SQL queries.");
    }
}