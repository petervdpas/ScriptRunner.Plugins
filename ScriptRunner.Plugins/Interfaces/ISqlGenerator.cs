using System;
using System.Collections.Generic;
using System.Data;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Defines methods for generating SQL queries and mapping <see cref="DataRow" /> values to SQL parameters.
/// </summary>
/// <remarks>
///     The <see cref="ISqlGenerator" /> interface provides functionality for creating SQL statements dynamically
///     based on a specified type and table name. It supports operations such as SELECT, INSERT, UPDATE, DELETE,
///     and mapping data rows to parameterize SQL queries.
/// </remarks>
public interface ISqlGenerator
{
    /// <summary>
    ///     Sets the dynamic type used to generate SQL queries.
    /// </summary>
    /// <param name="dynamicType">The dynamic class type representing the schema of the table.</param>
    /// <remarks>
    ///     The provided type should have a property representing the primary key, typically named "Id" or "ID".
    ///     This type is used to determine the columns for SQL operations.
    /// </remarks>
    void SetType(Type dynamicType);

    /// <summary>
    ///     Sets the name of the database table for SQL query generation.
    /// </summary>
    /// <param name="tableName">The name of the database table as a string.</param>
    /// <remarks>
    ///     This method must be called before generating any SQL queries to ensure the correct table is targeted.
    /// </remarks>
    void SetTableName(string tableName);

    /// <summary>
    ///     Generates a SQL SELECT query string for the specified table.
    /// </summary>
    /// <param name="filterById">
    ///     If <c>true</c>, the generated query includes a WHERE clause to filter results by the primary key (ID column).
    /// </param>
    /// <returns>A SQL SELECT query string.</returns>
    /// <remarks>
    ///     The SELECT query includes all columns from the table. When <paramref name="filterById" /> is true, the query
    ///     retrieves a single row based on the primary key.
    /// </remarks>
    string GenerateSelectQuery(bool filterById = false);

    /// <summary>
    ///     Generates an SQL INSERT query string using the properties of the dynamic type.
    /// </summary>
    /// <returns>A SQL INSERT query string with parameterized placeholders for each column.</returns>
    /// <remarks>
    ///     The generated query is designed to insert a new row into the table, with values corresponding to the properties
    ///     of the dynamic type.
    /// </remarks>
    string GenerateInsertQuery();

    /// <summary>
    ///     Generates an SQL UPDATE query string that updates columns in the table.
    /// </summary>
    /// <returns>A SQL UPDATE query string with a WHERE clause to filter by the primary key.</returns>
    /// <remarks>
    ///     The generated query updates one or more columns of a single row, identified by its primary key.
    ///     Values are parameterized for safe execution.
    /// </remarks>
    string GenerateUpdateQuery();

    /// <summary>
    ///     Generates a SQL DELETE query string for deleting a row from the table.
    /// </summary>
    /// <returns>A SQL DELETE query string with a WHERE clause to filter by the primary key.</returns>
    /// <remarks>
    ///     The DELETE query is parameterized to delete a specific row based on the primary key value.
    ///     This operation is generally used for single-row deletions.
    /// </remarks>
    string GenerateDeleteQuery();

    /// <summary>
    ///     Maps the values in a <see cref="DataRow" /> to a dictionary of SQL parameters.
    /// </summary>
    /// <param name="row">The <see cref="DataRow" /> containing the values to map.</param>
    /// <returns>
    ///     A dictionary where the keys are parameter names (prefixed with "@"),
    ///     and the values are the corresponding column values.
    /// </returns>
    /// <remarks>
    ///     The method handles type conversions (e.g., <see cref="DateTime" /> values are formatted as "yyyy-MM-dd")
    ///     and ensures SQL compatibility for parameterized queries.
    /// </remarks>
    Dictionary<string, object> MapParameters(DataRow row);
}