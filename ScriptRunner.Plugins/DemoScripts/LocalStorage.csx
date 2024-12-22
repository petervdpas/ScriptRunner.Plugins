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
