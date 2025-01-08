/*
{
    "TaskCategory": "Tools",
    "TaskName": "SqLiteDatabase [InMemory]",
    "TaskDetail": "Demonstrates the functionality of the SqLiteDatabase class (in memory variant)"
}
*/

// Get the SqLiteDatabase service from the DI container
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
