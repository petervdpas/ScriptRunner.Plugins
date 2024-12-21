using System.Collections.Generic;
using ScriptRunner.Plugins.Models;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Represents a generic database interface for executing commands, queries, and managing connections.
/// </summary>
public interface ISqliteDatabase : IDatabase
{
    /// <summary>
    /// Sets foreign key constraints in the SQLite database.
    /// </summary>
    void SetForeignKeysEnabled(bool enableForeignKeys = true);
    
    /// <summary>
    /// Loads entities from the database.
    /// </summary>
    /// <returns>A collection of entities.</returns>
    IEnumerable<Entity?> LoadEntities();

    /// <summary>
    /// Loads relationships from the database.
    /// </summary>
    /// <returns>A collection of relationships.</returns>
    IEnumerable<Relationship> LoadRelationships();
}