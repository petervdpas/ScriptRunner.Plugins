---
Title: Leveraging LocalStorage in Scripts  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, LocalStorage, ScriptRunner, Tools, Data Management]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true
---

# Recipe: Leveraging LocalStorage

## Goal

Demonstrate how to use the **LocalStorage** tool to store, retrieve, and manage temporary data with event hooks 
and time-to-live (TTL) functionality in ScriptRunner scripts.

---

## Steps

### 1. Write a Script

Create a ScriptRunner script that showcases the features of the **LocalStorage** tool. The script will demonstrate:
- Adding, updating, and removing data.
- Retrieving values with optional TTL functionality.
- Responding to data changes using event hooks.
- Persisting data to a file and restoring it later.

### 2. Run the Script

Execute the script in ScriptRunner to observe data management operations and event-driven behavior.

---

## Example Script

Below is an example script demonstrating the functionality of the **LocalStorage** tool:

```csharp
/*
{
    "TaskCategory": "Tools",
    "TaskName": "LocalStorageDemo",
    "TaskDetail": "Demonstrates the functionality of the LocalStorage class with event hooks and storage operations."
}
*/

var localStorage = new LocalStorage();

// Subscribe to events
localStorage.OnDataAdded += (key, value) => Console.WriteLine($"Data Added - Key: {key}, Value: {value}");
localStorage.OnDataUpdated += (key, value) => Console.WriteLine($"Data Updated - Key: {key}, Value: {value}");
localStorage.OnDataRemoved += (key) => Console.WriteLine($"Data Removed - Key: {key}");

// Add data to the storage
localStorage.SetData("Key1", "Value1");
localStorage.SetData("Key2", 42, TimeSpan.FromMinutes(5));
localStorage.SetData("Key3", true);

// Update data
localStorage.SetData("Key1", "UpdatedValue1");

// Retrieve and display data
var key1Value = localStorage.GetData<string>("Key1");
Dump($"Retrieved Key1 Value: {key1Value}");

// List all data
Dump("Listing all data:");
Dump(localStorage.ListAllData());

// Search for keys with a specific pattern
Dump("Keys matching pattern 'Key[1-2]':");
foreach (var key in localStorage.GetKeysMatching("Key[1-2]"))
{
    Dump(key);
}

// Remove a key
localStorage.RemoveData("Key2");

// Attempt to retrieve a removed key
var key2Value = localStorage.GetData<object>("Key2");
Dump($"Retrieved Key2 Value (should be null): {key2Value}");

// Save storage to a file
localStorage.SaveToFile("localstorage-demo.json");
Dump("Storage saved to file.");

// Clear all data
localStorage.ClearData();
Dump("Storage cleared.");

// Load storage from the file
localStorage.LoadFromFile("localstorage-demo.json");
Dump("Storage loaded from file.");

// Display storage after reloading
Dump("Listing all data after loading from file:");
Dump(localStorage.ListAllData());

return "LocalStorage demo completed.";
```  

---

## Explanation of the Script

1. **Data Management**:
    - Adds, updates, and removes data using **SetData**, **RemoveData**, and **ClearData**.
    - Retrieves values with the **GetData** method.

2. **Event Hooks**:
    - Demonstrates the **OnDataAdded**, **OnDataUpdated**, and **OnDataRemoved** hooks to track changes in storage.

3. **Time-to-Live (TTL)**:
    - Shows how to set a TTL for keys, after which they are automatically removed.

4. **Persistence**:
    - Saves the current storage state to a JSON file with **SaveToFile**.
    - Restores the storage state from a file using **LoadFromFile**.

5. **Key Search**:
    - Use **GetKeysMatching** to find keys matching a regular expression.

---

## What You Can Do Next

- **Dynamic Applications**: Use the LocalStorage tool for caching data in complex scripts.
- **Event-Driven Behavior**: Build workflows that respond to data changes using event hooks.
- **Data Persistence**: Extend the script to manage persistent configurations or session data.

---

This recipe demonstrates the flexibility of the **LocalStorage** tool in handling temporary data storage 
and event-driven operations. Experiment with it and adapt it to your scripting needs!  