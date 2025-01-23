---
Title: Using the DynamicClassGenerator
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, DynamicClassGenerator, ScriptRunner, JSON, Tools, C#]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true
---

# Recipe: Generating and Using Dynamic Classes with DynamicClassGenerator

## Goal

Demonstrate how to use the **DynamicClassGenerator** to dynamically generate and compile C# classes into an assembly 
using JSON input stored in a separate file.

---

## Steps

### 1. Create the JSON File

Save the following JSON definition in a file named **Person.dcg.json**. This file describes a namespace, 
a class (**Person**), its properties, constructors, and methods:

#### Person.dcg.json

```json
[
  {
    "Namespace": "DemoNamespace",
    "Usings": [
      "System"
    ],
    "Classes": [
      {
        "Name": "Person",
        "Properties": [
          {
            "Name": "Id",
            "Type": "int",
            "AccessorType": "set"
          },
          {
            "Name": "Name",
            "Type": "string",
            "AccessorType": "set"
          },
          {
            "Name": "Age",
            "Type": "int",
            "AccessorType": "set"
          }
        ],
        "Constructors": [
          {
            "Parameters": [],
            "BodyLines": [
              "Id = 0;",
              "Name = \"\";",
              "Age = 0;"
            ]
          },
          {
            "Parameters": [
              {
                "Name": "id",
                "Type": "int"
              },
              {
                "Name": "name",
                "Type": "string"
              },
              {
                "Name": "age",
                "Type": "int"
              }
            ],
            "BodyLines": [
              "Id = id;",
              "Name = name;",
              "Age = age;"
            ]
          }
        ],
        "Methods": [
          {
            "Name": "Greet",
            "ReturnType": "string",
            "Parameters": [],
            "BodyLines": [
              "return $\"Hello, my name is {Name} and I am {Age} years old.\";"
            ]
          }
        ]
      }
    ]
  }
]
```

### 2. Write the Script

Create the following ScriptRunner script to read the JSON file, generate the assembly, 
and use the dynamically created **Person** class:

#### ScriptRunner Script

```csharp
/*
{
    "TaskCategory": "Tools",
    "TaskName": "DynamicClassGeneratorDemo",
    "TaskDetail": "Demonstrates the functionality of the DynamicClassGenerator class for generating C# classes from JSON definitions and compiling them into an assembly.",
    "RequiredPlugins": []
}
*/

var fileHelper = new FileHelper();
string jsonFileName = fileHelper.RelativeToCurrentDirectory("Person.dcg.json");
string json = fileHelper.ReadFile(jsonFileName);

// Specify the output path for the generated DLL
string outputDllPath = "./Person.dcg.dll";

// Instantiate the DynamicClassGenerator with a logger
var logger = GetLogger("DynamicClassGenerator");
var generator = new DynamicClassGenerator(logger);

try
{
    // Generate the assembly from the JSON input
    var (assemblyPath, namespaces) = generator.GenerateAssemblyFromJson(json, outputDllPath);

    if (assemblyPath != null)
    {
        Dump($"Assembly successfully generated at: {assemblyPath}");
        Dump("Generated namespaces:");
        foreach (var ns in namespaces)
        {
            Dump($"- {ns}");
        }

        // Load the generated assembly
        var assembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(assemblyPath));
        Dump("Assembly loaded successfully!");

        // Get the Person type from the generated assembly
        var personType = assembly.GetType("DemoNamespace.Person");
        if (personType != null)
        {
            // Create an instance of the Person class
            dynamic person = Activator.CreateInstance(personType);
            person.Name = "John Doe";
            person.Age = 30;

            // Call the Greet method
            var greetMessage = person.Greet();
            Dump(greetMessage); // Output: "Hello, my name is John Doe and I am 30 years old."
        }
    }
    else
    {
        Dump("Assembly generation failed. Check the log for details.");
    }
}
catch (Exception ex)
{
    Dump($"An error occurred: {ex.Message}");
}

return "DynamicClassGenerator demo completed.";
```

---

## Explanation

1. **JSON Definition**:
    - The **Person.dcg.json** file defines a namespace (**DemoNamespace**) and a class (**Person**) with properties (**Id**, **Name**, **Age**), constructors, and a method (**Greet**).

2. **Script Workflow**:
    - **Step 1**: Read the JSON definition file using **FileHelper**.
    - **Step 2**: Generate a **.DLL** assembly using **DynamicClassGenerator**.
    - **Step 3**: Load the generated assembly dynamically.
    - **Step 4**: Create an instance of the **Person** class and invoke the **Greet** method.

3. **Dynamic Behavior**:
    - The script demonstrates the ability to dynamically compile and execute user-defined classes at runtime.

---

## What You Can Do Next

1. **Extend JSON Definitions**:
    - Add more classes, properties, methods, or inheritance to the JSON.

2. **Error Handling**:
    - Enhance the script with better error handling for invalid JSON or compilation errors.

3. **Dynamic Plugins**:
    - Use this approach to dynamically load plugins defined in JSON.

4. **Automation**:
    - Automate the generation of classes from different configurations.

---

This cookbook provides a step-by-step guide to using the **DynamicClassGenerator** for dynamic class creation 
and runtime compilation. Experiment and adapt this for your needs! 🚀
