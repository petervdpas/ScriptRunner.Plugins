# ScriptRunner Plugin: {{PluginProject}}

Welcome to your new ScriptRunner plugin project! This template provides the basic structure and setup needed to create a plugin for the **ScriptRunner** framework.

## Project Structure

The project is structured as follows:

```
{{PluginProject}}/
├── .template.config/         # Template configuration (for internal use in the NuGet package)
├── {{PluginProject}}.csproj  # Project file
├── README.md                 # This file
└── MyPlugin.cs               # Main plugin implementation
```

## Getting Started

1. **Build the Plugin:**
   Run the following command in the project directory to build the plugin:
   ```bash
   dotnet build
   ```

2. **Implement Plugin Logic:**
   Edit the `MyPlugin.cs` file to add your custom plugin logic.

3. **Package the Plugin:**
   Use `dotnet pack` to create a NuGet package if you'd like to distribute your plugin.

4. **Run the Plugin:**
   Add your plugin to the ScriptRunner plugin directory and load it into the application.

## Customization

- **Plugin Metadata:**
  Update the `[PluginMetadata]` attribute in `MyPlugin.cs` to reflect your plugin's information:
  ```csharp
    [PluginMetadata(
        name: "Your Plugin Name",
        description: "A plugin that provides ...",
        author: "YourName",
        version: "1.0.2",
        pluginSystemVersion: "1.0.18",
        frameworkVersion: ".NET 8.0",
        services: [])]
  ```

- **Dependencies:**
  If your plugin relies on additional libraries, add them as NuGet dependencies in the `.csproj` file:
  ```xml
  <ItemGroup>
    <PackageReference Include="SomeDependency" Version="1.0.0" />
  </ItemGroup>
  ```

## Notes

- Ensure your plugin adheres to the **ScriptRunner Plugin** guidelines.
- Check the [ScriptRunner documentation](https://example.com/docs) for advanced usage and examples.

## License

This plugin is distributed under the [MIT License](LICENSE).
