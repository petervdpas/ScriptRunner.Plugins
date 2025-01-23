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
string outputDllPath = "./Test.dll";

// Instantiate the DynamicClassGenerator with a logger
var logger = GetLogger("DynamicClassGenerator");
var generator = new DynamicClassGenerator(logger);

try
{
    // Generate the assembly from the JSON input
    var (assemblyPath, namespaces) = generator.GenerateAssemblyFromJson(json, outputDllPath);

    if (assemblyPath != null)
    {
        Console.WriteLine($"Assembly successfully generated at: {assemblyPath}");
        Console.WriteLine("Generated namespaces:");
        foreach (var ns in namespaces)
        {
            Console.WriteLine($"- {ns}");
        }

        // Load the generated assembly
        var assembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(assemblyPath));
        Console.WriteLine("Assembly loaded successfully!");

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
            Console.WriteLine(greetMessage); // Output: "Hello, my name is John Doe."
        }
    }
    else
    {
        Console.WriteLine("Assembly generation failed. Check the log for details.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"An error occurred: {ex.Message}");
}

return "DynamicClassGenerator demo completed.";