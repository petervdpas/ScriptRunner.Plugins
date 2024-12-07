# ScriptRunner.Plugins

[![NuGet](https://img.shields.io/nuget/v/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![License](https://img.shields.io/github/license/petervdpas/ScriptRunner.Plugins.svg)](https://opensource.org/licenses/MIT)

`ScriptRunner.Plugins` provides core interfaces, attributes, and utilities for building and integrating plugins into the `ScriptRunner` framework. It simplifies plugin development and ensures compatibility with the ScriptRunner ecosystem.

## Features

- **Plugin Metadata Attribute**: Annotate your plugin classes with metadata.
- **Service Plugins**: Support for registering services into dependency injection containers.
- **Custom Initialization**: Plugins can define custom initialization logic.

## Installation

You can install the NuGet package using the `.NET CLI` or `Package Manager` in Visual Studio.

```bash
dotnet add package ScriptRunner.Plugins
```

## Usage

### Creating a Plugin

1. **Define a plugin class**:
   Annotate your class with `[PluginMetadata]` to provide details like the name, description, and version of the plugin.

   ```csharp
   using ScriptRunner.Plugins.Abstractions;
   using ScriptRunner.Plugins.Abstractions.Attributes;

   [PluginMetadata(
       name: "ExamplePlugin",
       description: "A sample plugin for ScriptRunner",
       author: "Your Name",
       version: "1.0.0",
       frameworkVersion: "net8.0")]
   public class ExamplePlugin : IPlugin
   {
       public void Initialize(IDictionary<string, object>? configuration)
       {
           // Plugin initialization logic
           Console.WriteLine("ExamplePlugin initialized.");
       }
   }
   ```

2. **Register services (optional)**:
   If your plugin provides services, implement `IServicePlugin`.

   ```csharp
   public class ExamplePlugin : IPlugin, IServicePlugin
   {
       public void Initialize(IDictionary<string, object>? configuration)
       {
           // Initialization logic
       }

       public void RegisterServices(IServiceCollection services)
       {
           services.AddSingleton<IMyService, MyService>();
       }
   }
   ```

### Loading Plugins in ScriptRunner

`ScriptRunner` automatically discovers and loads plugins placed in the designated plugin directory. Ensure your plugin assembly is available in the directory.

## Contributing

Contributions are welcome! If you find a bug or have a feature request, please open an issue or submit a pull request on [GitHub](https://github.com/your-repo/ScriptRunner.Plugins).

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).

