/*
{
    "TaskCategory": "Extensions",
    "TaskName": "JsonExtensions",
    "TaskDetail": "A demo script showcasing JSON extension methods",
    "RequiredPlugins": []
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
