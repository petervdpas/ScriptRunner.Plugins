---
Title: Dynamically Generating SQL Queries with SqlGenerator
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, SqlGenerator, ScriptRunner, Tools, SQL, Dynamic Queries]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true
---

# Recipe: Dynamically Generating SQL Queries with SqlGenerator

## Goal

Demonstrate how to use the **SqlGenerator** class to dynamically generate SQL queries and map parameters for 
database operations in ScriptRunner scripts.

---

## Steps

### 1. Understand the SqlGenerator Class

The **SqlGenerator** class dynamically generates SQL queries (**SELECT**, **INSERT**, **UPDATE**, **DELETE**) 
based on a provided .NET type and table name. It also supports mapping **DataRow** values to SQL parameters 
for secure query execution.

---

### 2. Create a Script

Develop a ScriptRunner script that uses the **SqlGenerator** class to:

- Define a dynamic type and table name.
- Generate SQL queries for common operations.
- Map parameters for queries based on a **DataRow**.
- Execute the generated queries using a database.

---

## Example Script

Below is a script demonstrating the use of the **SqlGenerator** class:

```csharp
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
```

---

## Explanation of the Script

1. **Dynamic Type and Table Setup**:
    - The **User** class is used to define the table schema (**Id**, **Name**, **Address**).
    - The table name is specified using **SetTableName**.

2. **SQL Query Generation**:
    - Queries like **SELECT**, **INSERT**, **UPDATE**, and **DELETE** are generated dynamically based on the type and table name.

3. **Parameter Mapping**:
    - The **MapParameters** method maps a **DataRow** to a dictionary of SQL parameters. It ensures secure, parameterized queries to prevent SQL injection.

4. **Database Integration**:
    - The generated queries are executed with the **SqliteDatabase** class to interact with a SQLite database.

---

## Example Output

When you run the script, you will see:

1. Generated SQL queries for the **Users** table.
2. Mapped parameters for the sample **DataRow**.
3. Users' table contents fetched from the SQLite database.

---

## What You Can Do Next

- **Extend the Schema**:
    - Add more properties to the dynamic type and observe how the queries adapt.

- **Complex Queries**:
    - Modify the script to generate and execute more complex queries (e.g., joins).

- **Database Operations**:
    - Use the script to manage other tables in your database.

---

This cookbook demonstrates the power of **SqlGenerator** for dynamically creating SQL queries and 
managing database operations. Adapt and expand this recipe for your dynamic database needs!  