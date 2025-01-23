---
Title: Utilizing the SqliteDatabase Class in Scripts
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, LocalStorage, SqliteDatabase, Tools, Data Management]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true
---

# Recipe: Utilizing the SqliteDatabase Class

## Goal

Demonstrate how to use the **SqliteDatabase** tool to manage SQLite databases, including setting up a connection, 
executing queries, and working with in-memory or file-based databases in ScriptRunner scripts.

---

## Steps

### 1. Write a Script

Create a ScriptRunner script that showcases the features of the **SqliteDatabase** class. The script will demonstrate:

- Setting up and opening SQLite database connections.
- Executing SQL commands for creating, modifying, and querying tables.
- Handling both in-memory and file-based databases.

### 2. Run the Script

Execute the script in ScriptRunner to observe database operations and the management of SQLite databases.

---

## Example Script: In-Memory SQLite Database

Below is an example script demonstrating the functionality of the **SqliteDatabase** class for an in-memory database:

```csharp
/*
{
    "TaskCategory": "Tools",
    "TaskName": "SqliteDatabase [InMemory]",
    "TaskDetail": "Demonstrates the functionality of the SqliteDatabase class (in-memory variant)."
}
*/

// Get the SqliteDatabase service from the DI container
var db = new SqliteDatabase();

// Set up the connection string for the in-memory database
db.Setup("Data Source=:memory:");

// Open the database connection
db.OpenConnection();

// Create a Users table
string createTableQuery = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Address TEXT NOT NULL
);";
db.ExecuteNonQuery(createTableQuery);

// Insert some records into the Users table
string insertUsersQuery = @"
INSERT INTO Users (Name, Address) VALUES 
('John Doe', '123 Elm Street'),
('Jane Smith', '456 Oak Avenue');
";
db.ExecuteNonQuery(insertUsersQuery);

// Query the count of users
string query = "SELECT COUNT(*) FROM Users";
var result = db.ExecuteScalar(query);
Dump($"Number of users: {result}");

// Fetch all records from Users table
var usersTable = db.ExecuteQuery("SELECT * FROM Users");
DumpTable("Users:", usersTable);

// Close the database connection
db.CloseConnection();

return $"Task completed successfully with {result} users.";
```

---

## Example Script: File-Based SQLite Database

Below is another example script demonstrating the **SqliteDatabase** class with a file-based database:

```csharp
/*
{
    "TaskCategory": "Tools",
    "TaskName": "SqliteDatabase [FileBased]",
    "TaskDetail": "Demonstrates the functionality of the SqliteDatabase class (file-based variant)."
}
*/

var fileHelper = new FileHelper();
fileHelper.EnsureDirectory("./data");

// Get the SqliteDatabase service from the DI container
var db = new SqliteDatabase();

// Set up the connection string for the file-based database
var dbFilePath = "data/MyDatabase.db";
db.Setup($"Data Source={dbFilePath};");

// Open the database connection
db.OpenConnection();

// Create a Users table
string createTableQuery = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Address TEXT NOT NULL
);";
db.ExecuteNonQuery(createTableQuery);

// Truncate (delete all data) from the Users table
string truncateTableQuery = "DELETE FROM Users;";
db.ExecuteNonQuery(truncateTableQuery);

// Reset the AUTOINCREMENT counter
string resetAutoincrementQuery = "DELETE FROM sqlite_sequence WHERE name = 'Users';";
db.ExecuteNonQuery(resetAutoincrementQuery);

// Insert some records into the Users table
string insertUsersQuery = @"
INSERT INTO Users (Name, Address) VALUES 
('John Doe', '123 Elm Street'),
('Jane Smith', '456 Oak Avenue');
";
db.ExecuteNonQuery(insertUsersQuery);

// Query the count of users
string query = "SELECT COUNT(*) FROM Users";
var result = db.ExecuteScalar(query);
Dump($"Number of users: {result}");

// Fetch all records from Users table
var usersTable = db.ExecuteQuery("SELECT * FROM Users");
DumpTable("Users:", usersTable);

// Close the database connection
db.CloseConnection();

return $"Task completed successfully with {result} users.";
```

---

## Explanation of the Scripts

1. **Database Setup**:
    - **Setup** initializes the database connection string (in-memory or file-based).

2. **Connection Management**:
    - Use **OpenConnection** and **CloseConnection** to manage the database connection.

3. **SQL Execution**:
    - Execute SQL commands with **ExecuteNonQuery**, **ExecuteScalar**, and **ExecuteQuery**.

4. **Data Management**:
    - The scripts show how to create tables, insert data, truncate data, and query results.

5. **In-Memory vs. File-Based**:
    - Use **Data Source=:memory:** for in-memory databases.
    - Specify a file path for file-based databases.

---

## What You Can Do Next

- **Dynamic Applications**: Use **SqliteDatabase** for managing application data or caching.
- **Custom Queries**: Extend the scripts to execute complex queries and joins.
- **File-Based Persistence**: Manage data across script runs using file-based databases.

---

This recipe demonstrates the flexibility of the **SqliteDatabase** class in managing SQLite databases. 
Experiment with it and adapt it to your scripting needs!

