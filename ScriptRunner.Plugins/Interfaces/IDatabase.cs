﻿using System.Collections.Generic;
using System.Data;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a generic database interface for executing commands, queries, and managing connections.
/// </summary>
public interface IDatabase
{
    /// <summary>
    ///     Sets up the database with a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to initialize the database.</param>
    void Setup(string connectionString);

    /// <summary>
    ///     Opens a database connection.
    /// </summary>
    void OpenConnection();

    /// <summary>
    ///     Closes the current database connection.
    /// </summary>
    void CloseConnection();

    /// <summary>
    ///     Retrieves the underlying database connection.
    /// </summary>
    /// <returns>The active <see cref="IDbConnection" />.</returns>
    IDbConnection GetConnection();

    /// <summary>
    ///     Executes a non-query command (INSERT, UPDATE, DELETE, etc.).
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);

    /// <summary>
    ///     Executes a scalar query and returns a single value.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The result of the scalar query.</returns>
    object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null);

    /// <summary>
    ///     Executes a query and returns the results as a DataTable.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>A DataTable containing the query results.</returns>
    DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null);
}