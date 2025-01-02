---
Title: Exploring DataTable Extensions  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, DataTableExtensions, ScriptRunner, DataTable]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true  
---

# Recipe: Exploring DataTable Extensions

## Goal

Demonstrate the usage of the **DataTableExtensions** library to perform various operations on **DataTable** objects in
ScriptRunner scripts.

---

## Steps

### 1. Write a Script

Create a ScriptRunner script that uses the **DataTableExtensions** library. The script will showcase operations
such as filtering rows, sorting data, adding new rows, exporting to CSV, and more.

### 2. Run the Script

Execute the script in ScriptRunner to observe the results of each operation.

---

## Example Script

Below is an example script that demonstrates the functionality of the **DataTableExtensions** library:

```csharp
/*
{
    "TaskCategory": "Extensions",
    "TaskName": "DataTableExtensionsDemo",
    "TaskDetail": "A demo script showcasing DataTable extension methods"
}
*/

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

return "DataTableExtensions demo completed.";
```

---

## Explanation of the Script

1. **Dump**: Outputs a human-readable representation of the **DataTable** using **Dump**.
2. **Filter**: Filters rows based on a predicate using **Filter**, e.g., rows where age > 25.
3. **Sort**: Sorts rows by a specified column using **Sort**.
4. **AddRow**: Adds a new row with specified values using **AddRow**.
5. **ToCsv**: Exports the content of the **DataTable** to a CSV string using **ToCsv**.
6. **IsEmpty**: Checks if the **DataTable** has no rows using **IsEmpty**.
7. **GetDistinctRows**: Retrieves distinct rows based on specific columns using **GetDistinctRows**.

---

## What You Can Do Next

1. **Extend the Script**:
    - Add more operations such as merging tables or handling missing data.

2. **Dynamic Inputs**:
    - Modify the script to accept user inputs for dynamic filtering or sorting criteria.

3. **Integrate with Other Extensions**:
    - Combine **DataTableExtensions** with other extensions for advanced scenarios, such as exporting to Excel or
      generating reports.

---

This recipe demonstrates how to leverage the **DataTableExtensions** library to simplify and enhance **DataTable**
operations in ScriptRunner. Start using these powerful extensions to handle all your data table needs efficiently!

