using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins;

/// <summary>
/// Provides methods for managing an SQLite database connection and executing various SQL commands.
/// </summary>
/// <remarks>
/// This class is designed to work specifically with SQLite databases. 
/// It supports basic database operations such as querying, inserting, updating, and deleting data, 
/// as well as schema exploration (e.g., loading table metadata and foreign key relationships).
/// 
/// Key features include:
/// - Connection management (open, close, and configure connections).
/// - Query execution with parameterized commands to prevent SQL injection.
/// - Schema inspection for tables and relationships.
/// - Support for enabling foreign key constraints.
///
/// Note: The database connection must be properly set up and opened before executing commands.
/// </remarks>
public class SqLiteDatabase : IDatabase
{
    private SqliteConnection? _connection;
    private string? _connectionString;

    /// <summary>
    ///     Sets up the database by providing the necessary connection string.
    ///     This method must be called before attempting to open a connection or
    ///     execute queries on the database.
    /// </summary>
    /// <param name="connectionString">The connection string used to establish a connection to the SQLite database.</param>
    /// <remarks>
    ///     The connection string specifies how to connect to the SQLite database, and it can include parameters such as
    ///     the data source and other connection-related settings.
    ///     If this method is not called before other operations, an exception will be thrown when attempting to open the
    ///     connection.
    /// </remarks>
    public void Setup(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    ///     Opens the database connection, with an option to enable foreign key constraints.
    /// </summary>
    /// <param name="enableForeignKeys">
    ///     A boolean indicating whether to enable foreign key constraints (default is true).
    /// </param>
    public void OpenConnection(bool enableForeignKeys = true)
    {
        SafeGuard();

        if (_connection != null && _connection.State != ConnectionState.Closed) return;

        _connection = new SqliteConnection(_connectionString);
        _connection.Open();

        if (!enableForeignKeys) return;

        // Enable foreign key constraints
        using var command = _connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_keys = ON;";
        command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Closes the database connection.
    /// </summary>
    public void CloseConnection()
    {
        SafeGuard();

        if (_connection?.State == ConnectionState.Open) _connection.Close();
    }

    /// <summary>
    ///     Checks if foreign key constraints are enabled for the current SQLite database connection.
    /// </summary>
    /// <returns>
    ///     A boolean value indicating whether foreign key constraints are enabled:
    ///     <c>true</c> if enabled, <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    ///     SQLite does not enforce foreign key constraints unless they are explicitly enabled using
    ///     the <c>PRAGMA foreign_keys = ON;</c> command. This method verifies the current state
    ///     of the foreign key configuration for the active database connection.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the database connection is not properly initialized or is not open.
    /// </exception>
    public bool AreForeignKeysEnabled()
    {
        SafeGuard();
        using var command = new SqliteCommand("PRAGMA foreign_keys;", _connection);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    /// <summary>
    ///     Executes a non-query SQL command (e.g., INSERT, UPDATE, DELETE) with optional parameters.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">A dictionary of parameter names and values to bind to the query.</param>
    /// <returns>The number of rows affected.</returns>
    public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
    {
        SafeGuard();

        using var command = parameters != null
            ? CreateCommandWithParameters(query, parameters)
            : new SqliteCommand(query, _connection);

        return command.ExecuteNonQuery();
    }

    /// <summary>
    ///     Executes an SQL query that returns a single value.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="parameters">Optional query parameters.</param>
    /// <returns>The value returned by the query, or null if no value is found.</returns>
    public object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
    {
        SafeGuard();

        if (_connection is not { State: ConnectionState.Open })
            throw new InvalidOperationException("The database connection is not open.");

        using var command = parameters != null
            ? CreateCommandWithParameters(query, parameters)
            : new SqliteCommand(query, _connection);

        return command.ExecuteScalar();
    }

    /// <summary>
    ///     Executes a SQL query and returns the result as a <see cref="DataTable" />.
    /// </summary>
    /// <param name="query">The SQL query to execute. The query must be a valid SQL SELECT statement.</param>
    /// <param name="parameters">
    /// A dictionary containing parameter names and their values to bind to the query. 
    /// Use this to safely include dynamic values in the query to prevent SQL injection. 
    /// Pass <c>null</c> if no parameters are required.
    /// </param>
    /// <returns>A <see cref="DataTable" /> containing the result set of the query.</returns>
    public DataTable ExecuteQuery(string query, Dictionary<string, object>? parameters = null)
    {
        SafeGuard();

        if (_connection is not { State: ConnectionState.Open })
            throw new InvalidOperationException("The database connection is not open.");

        var dataTable = new DataTable();
        using var command = new SqliteCommand(query, _connection);

        // Bind parameters if provided
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        using var reader = command.ExecuteReader();

        // Load the data from the reader into the DataTable
        dataTable.Load(reader);
        return dataTable;
    }

    /// <summary>
    ///     Loads a collection of entities (tables) from the SQLite database.
    /// </summary>
    /// <remarks>
    ///     Since SQLite does not support <c>INFORMATION_SCHEMA</c>, table names are obtained using
    ///     the <c>sqlite_master</c> system table.
    /// </remarks>
    /// <returns>
    ///     A collection of <see cref="Entity" /> objects, each representing a table and its columns in the database.
    /// </returns>
    public IEnumerable<Entity?> LoadEntities(string? schema = null, string? queryOverwrite = null, string? cleaningToken = null)
    {
        SafeGuard();

        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine("SELECT name FROM sqlite_master");
        queryBuilder.AppendLine("WHERE type = 'table'");

        var query = queryBuilder.ToString();
        var dataTable = ExecuteQuery(query);

        return from DataRow row in dataTable.Rows
            select row["name"].ToString() ?? string.Empty
            into tableName
            select new Entity(tableName, LoadTableColumns(tableName));
    }

    /// <summary>
    ///     Loads foreign key relationships between entities (tables) in the SQLite database.
    /// </summary>
    /// <remarks>
    ///     SQLite does not have an equivalent to SQL Server's foreign key representation
    ///     in <c>INFORMATION_SCHEMA</c>.
    ///     Instead, the <c>PRAGMA foreign_key_list</c> command is
    ///     used to gather information about foreign keys for each table.
    /// </remarks>
    /// <returns>
    ///     A collection of <see cref="Relationship" /> objects, each representing a foreign key relationship between tables.
    /// </returns>
    public IEnumerable<Relationship> LoadRelationships(string? schema = null, string? queryOverwrite = null, string? cleaningToken = null)
    {
        SafeGuard();

        // Note: SQLite does not have an equivalent to SQL Server's foreign key representation in INFORMATION_SCHEMA.
        // Instead, PRAGMA foreign_key_list can be used to gather information about foreign keys for each table.

        var relationships = new List<Relationship>();

        const string tablesQuery = "SELECT name FROM sqlite_master WHERE type = 'table'";
        var tablesDataTable = ExecuteQuery(tablesQuery);

        foreach (DataRow row in tablesDataTable.Rows)
        {
            var tableName = row["name"].ToString() ?? string.Empty;

            var foreignKeysQuery = $"PRAGMA foreign_key_list({tableName})";
            var foreignKeysDataTable = ExecuteQuery(foreignKeysQuery);

            relationships.AddRange(from DataRow fkRow in foreignKeysDataTable.Rows
                select new Relationship
                {
                    FromEntity = tableName,
                    ToEntity = fkRow["table"].ToString() ?? string.Empty,
                    Key = fkRow["from"].ToString() ?? string.Empty
                });
        }

        return relationships;
    }

    /// <summary>
    ///     Loads the columns and their metadata for a given table in the SQLite database.
    /// </summary>
    /// <remarks>
    ///     SQLite does not have <c>INFORMATION_SCHEMA</c>, so the <c>PRAGMA table_info</c> command is used
    ///     to retrieve the column names, types, and nullability information.
    /// </remarks>
    /// <param name="tableName">The name of the table for which to load columns.</param>
    /// <returns>
    ///     A dictionary where the keys are column names and the values are dictionaries containing
    ///     metadata about each column, such as the data type and whether it is nullable.
    /// </returns>
    private Dictionary<string, object> LoadTableColumns(string tableName)
    {
        SafeGuard();

        // SQLite does not have INFORMATION_SCHEMA, so we use PRAGMA table_info instead
        var query = $"PRAGMA table_info({tableName})";
        var dataTable = ExecuteQuery(query);

        var attributes = new Dictionary<string, object>();

        foreach (DataRow row in dataTable.Rows)
        {
            var columnName = row["name"].ToString() ?? string.Empty;
            var columnType = row["type"].ToString() ?? string.Empty;
            var isNullable = row["notnull"].ToString() == "0"; // 0 means nullable in SQLite its PRAGMA table_info

            var fieldAttributes = new Dictionary<string, object>
            {
                { "Type", columnType },
                { "IsNullable", isNullable }
            };

            attributes.Add(columnName, fieldAttributes);
        }

        return attributes;
    }

    /// <summary>
    ///     Creates and configures a SQLite command with the specified query and parameters.
    /// </summary>
    /// <param name="query">The SQL query with placeholders for parameters (e.g., @paramName).</param>
    /// <param name="parameters">A dictionary of parameter names and values.</param>
    /// <returns>A configured <see cref="SqliteCommand" /> with parameters.</returns>
    private SqliteCommand CreateCommandWithParameters(string query, Dictionary<string, object> parameters)
    {
        SafeGuard();

        var command = new SqliteCommand(query, _connection);
        foreach (var parameter in parameters)
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);

        return command;
    }

    /// <summary>
    ///     Ensures that the connection string for the SQLite database is set.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the connection string has not been set before attempting to access the database.
    /// </exception>
    private void SafeGuard()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException("The connection string has not been set.");
    }
}