/*
{
    "TaskCategory": "Tools",
    "TaskName": "SqLiteDatabase [FileBased]",
    "TaskDetail": "Demonstrates the functionality of the SqLiteDatabase class (file-based variant)",
    "RequiredPlugins": []
}
*/

var fileHelper = new FileHelper();
fileHelper.EnsureDirectory("./data");

// Get the SqLiteDatabase service from the DI container
var db = new SqliteDatabase();

// Set up the connection string for the in-memory database
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
