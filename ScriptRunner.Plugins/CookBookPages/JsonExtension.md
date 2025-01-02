---
Title: Exploring JSON Extensions  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, JsonExtensions, ScriptRunner, JSON]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true  
---

# Recipe: Exploring JSON Extensions

## Goal

Demonstrate the usage of the **JsonExtensions** library to perform JSON serialization, deserialization, and reformatting
in ScriptRunner scripts.

---

## Steps

### 1. Write a Script

Create a ScriptRunner script that uses the **JsonExtensions** library.  
The script will showcase operations like converting objects to JSON, reformatting JSON strings, and deserializing JSON
back into objects.

### 2. Run the Script

Execute the script in ScriptRunner to observe the JSON manipulations and their results.

---

## Example Script

Below is an example script that demonstrates the functionality of the **JsonExtensions** library:

```csharp
/*
{
    "TaskCategory": "JSONManipulation",
    "TaskName": "JsonExtensionsDemo",
    "TaskDetail": "A demo script showcasing JSON extension methods"
}
*/

var sampleObject = new
{
    Name = "John Doe",
    Age = 30,
    Occupation = "Software Engineer",
    Skills = new[] { "C#", "JavaScript", "SQL" }
};

// Convert object to JSON
var jsonString = sampleObject.ToJson();
Dump("Serialized JSON:");
Dump(jsonString);

// Reformat JSON
var compactJson = "{\"Name\":\"Jane Doe\",\"Age\":25,\"Occupation\":\"Data Scientist\"}";
var prettyJson = compactJson.ReformatJson();
Dump("Reformatted JSON:");
Dump(prettyJson);

// Deserialize JSON to an object
var jsonInput = @"
{
    ""Name"": ""Alice"",
    ""Age"": 28,
    ""Occupation"": ""Designer"",
    ""Skills"": [""Photoshop"", ""Illustrator"", ""Figma""]
}";
var deserializedObject = jsonInput.FromJson<dynamic>();
Dump("Deserialized Object:");
Dump($"Name: {deserializedObject.Name}, Age: {deserializedObject.Age}, Occupation: {deserializedObject.Occupation}");

// Invalid JSON Example (Error Handling)
try
{
    var invalidJson = "{Name: \"Bob\" Age: 35}";
    invalidJson.ReformatJson();
}
catch (Exception ex)
{
    Dump("Error while reformatting invalid JSON:");
    Dump(ex.Message);
}

return "JsonExtensions demo completed.";
```  

---

## Explanation of the Script

1. **Serialize an Object**:
    - Uses the **ToJson** method to convert an object into a JSON string.
    - Example:
      ```json
      {
          "Name": "John Doe",
          "Age": 30,
          "Occupation": "Software Engineer",
          "Skills": ["C#", "JavaScript", "SQL"]
      }
      ```  

2. **Reformat JSON**:
    - Uses the **ReformatJson** method to convert compact JSON into a pretty-printed format.

3. **Deserialize JSON**:
    - Uses the **FromJson** method to deserialize a JSON string into a dynamic object.
    - Example output: **Name: Alice, Age: 28, Occupation: Designer**.

4. **Error Handling**:
    - Demonstrates how invalid JSON input raises an exception in the **ReformatJson** method.

---

## What You Can Do Next

1. **Dynamic JSON Generation**:
    - Modify the script to generate JSON dynamically based on user input.

2. **Integrate with APIs**:
    - Use these methods to process API responses and prepare request payloads.

3. **Error Resilience**:
    - Add custom error handling for JSON operations to improve script robustness.

---

This recipe demonstrates the powerful JSON manipulation capabilities of the **JsonExtensions** library in ScriptRunner.
Use these methods to simplify JSON-related tasks in your scripts!
