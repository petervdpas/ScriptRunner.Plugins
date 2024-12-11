using System.Collections.Generic;
using System.Data;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Represents a generic database interface for executing commands, queries, and managing connections.
/// </summary>
public interface IDatabase
{
    /// <summary>
    /// Sets up the database with a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to initialize the database.</param>
    void Setup(string connectionString);

    /// <summary>
    /// Opens a database connection.
    /// </summary>
    /// <param name="enableForeignKeys">Specifies whether to enable foreign keys (if supported).</param>
    void OpenConnection(bool enableForeignKeys = true);

    /// <summary>
    /// Closes the current database connection.
    /// </summary>
    void CloseConnection();

    /// <summary>
    /// Executes a non-query command (INSERT, UPDATE, DELETE, etc.).
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The number of rows affected.</returns>
    int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);

    /// <summary>
    /// Executes a scalar query and returns a single value.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The result of the scalar query.</returns>
    object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null);

    /// <summary>
    /// Executes a query and returns the results as a DataTable.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>A DataTable containing the query results.</returns>
    DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null);

    /// <summary>
    /// Loads entities from the database.
    /// </summary>
    /// <param name="schema">Optional schema for filtering results.</param>
    /// <param name="queryOverwrite">Optional custom query to override the default.</param>
    /// <param name="cleaningToken">Optional token for cleaning or filtering results.</param>
    /// <returns>A collection of entities.</returns>
    IEnumerable<Entity?> LoadEntities(string? schema = null, string? queryOverwrite = null, string? cleaningToken = null);

    /// <summary>
    /// Loads relationships from the database.
    /// </summary>
    /// <param name="schema">Optional schema for filtering results.</param>
    /// <param name="queryOverwrite">Optional custom query to override the default.</param>
    /// <param name="cleaningToken">Optional token for cleaning or filtering results.</param>
    /// <returns>A collection of relationships.</returns>
    IEnumerable<Relationship> LoadRelationships(string? schema = null, string? queryOverwrite = null, string? cleaningToken = null);
}