/*
{
    "TaskCategory": "Tools",
    "TaskName": "SqlGeneratorDemo",
    "TaskDetail": "Demonstrates the functionality of the SqlGenerator class for dynamic SQL generation and parameter mapping.",
    "RequiredPlugins": []
}
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

// Define a dynamic type representing the schema of a table
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

// Instantiate SqlGenerator
var sqlGenerator = new SqlGenerator();

// Set the dynamic type and table name
sqlGenerator.SetType(typeof(User));
sqlGenerator.SetTableName("Users");

// Generate SQL queries dynamically
var selectQuery = sqlGenerator.GenerateSelectQuery();
var selectByIdQuery = sqlGenerator.GenerateSelectQuery(filterById: true);
var insertQuery = sqlGenerator.GenerateInsertQuery();
var updateQuery = sqlGenerator.GenerateUpdateQuery();
var deleteQuery = sqlGenerator.GenerateDeleteQuery();

// Print generated SQL queries
Dump("Generated SQL Queries:");
Dump($"SELECT Query: {selectQuery}");
Dump($"SELECT Query with WHERE clause: {selectByIdQuery}");
Dump($"INSERT Query: {insertQuery}");
Dump($"UPDATE Query: {updateQuery}");
Dump($"DELETE Query: {deleteQuery}");

// Simulate a DataTable with sample data
var dataTable = new DataTable();
dataTable.Columns.Add("Id", typeof(int));
dataTable.Columns.Add("Name", typeof(string));
dataTable.Columns.Add("Address", typeof(string));

// Add a sample row
var row = dataTable.NewRow();
row["Id"] = 1;
row["Name"] = "John Doe";
row["Address"] = "123 Elm Street";
dataTable.Rows.Add(row);

// Map parameters from the DataRow
var parameters = sqlGenerator.MapParameters(row);

// Print mapped parameters
Dump("Mapped Parameters from DataRow:");
foreach (var param in parameters)
{
    Dump($"{param.Key}: {param.Value}");
}

// Example: Use the generated queries and mapped parameters with a database (pseudo code)
var db = new SqliteDatabase();
db.Setup("Data Source=:memory:");

// Open the database connection
db.OpenConnection();

// Create the Users table
db.ExecuteNonQuery(@"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Address TEXT NOT NULL
);");

// Insert the sample row into the database
db.ExecuteNonQuery(insertQuery, parameters);

// Fetch and display all rows
var usersTable = db.ExecuteQuery(selectQuery);
DumpTable("Users Table:", usersTable);

// Close the database connection
db.CloseConnection();

return "SqlGenerator demo completed.";
