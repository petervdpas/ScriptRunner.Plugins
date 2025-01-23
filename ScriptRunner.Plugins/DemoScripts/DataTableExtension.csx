/*
{
    "TaskCategory": "Extensions",
    "TaskName": "DataTableExtensionsDemo",
    "TaskDetail": "A demo script showcasing DataTable extension methods",
    "RequiredPlugins": []
}
*/

// Helper class for collection-to-DataTable demo
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

var dataTable = new DataTable();

// Define columns
dataTable.Columns.Add("Id", typeof(int));
dataTable.Columns.Add("Name", typeof(string));
dataTable.Columns.Add("Age", typeof(int));

// Add rows
dataTable.Rows.Add(1, "Alice", 25);
dataTable.Rows.Add(2, "Bob", 30);
dataTable.Rows.Add(3, "Charlie", 35);
dataTable.Rows.Add(4, "Diana", 28);
dataTable.Rows.Add(5, "Alice", 25);

// Dump table
var dumpedTable = dataTable.Dump("Original DataTable");
Dump(dumpedTable);

// Filter rows
var filteredTable = dataTable.Filter(row => (int)row["Age"] > 25);
var filteredDump = filteredTable.Dump("Filtered Rows (Age > 25)");
Dump(filteredDump);

// Sort rows
var sortedTable = dataTable.Sort("Name", ascending: true);
var sortedDump = sortedTable.Dump("Sorted Rows by Name");
Dump(sortedDump);

// Add a new row
var newRow = new Dictionary<string, object>
{
    { "Id", 6 },
    { "Name", "Eve" },
    { "Age", 22 }
};
dataTable.AddRow(newRow);
var updatedDump = dataTable.Dump("Updated DataTable (New Row Added)");
Dump(updatedDump);

// Export to CSV
var csvData = dataTable.ToCsv();
Dump($"CSV Export:\n{csvData}");

// Check if table is empty
var isEmpty = dataTable.IsEmpty();
Dump($"Is DataTable Empty: {isEmpty}");

// Get distinct rows
var distinctTable = dataTable.GetDistinctRows("Name", "Age");
var distinctDump = distinctTable.Dump("Distinct Rows by Name and Age");
Dump(distinctDump);

// Convert collection to DataTable
var users = new List<User>
{
    new User { Id = 1, Name = "Alice", Age = 25 },
    new User { Id = 2, Name = "Bob", Age = 30 },
    new User { Id = 3, Name = "Charlie", Age = 35 }
};
var userTable = users.ToDataTable();
var userDump = userTable.Dump("DataTable from Collection");
Dump(userDump);

return "DataTableExtensions demo completed.";
