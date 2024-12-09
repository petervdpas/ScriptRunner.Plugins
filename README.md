# ScriptRunner.Plugins

[![NuGet](https://img.shields.io/nuget/v/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![License](https://img.shields.io/github/license/petervdpas/ScriptRunner.Plugins.svg)](https://opensource.org/licenses/MIT)


`ScriptRunner.Plugins` provides core interfaces, attributes, and utilities for building and integrating plugins into the `ScriptRunner` framework. It simplifies plugin development and ensures compatibility with the ScriptRunner ecosystem.

---

## Features

- **Plugin Metadata Attribute**: Add metadata to your plugin classes using `[PluginMetadata]`.
- **Dependency Injection Support**: Register plugin services into the DI container with `IServicePlugin`.
- **Plugin Dependencies**: Declare and validate dependencies between plugins.
- **Plugin System Versioning**: Automatically validate plugin compatibility with the current plugin system version.
- **Dynamic Plugin Discovery**: Discover and validate plugins at runtime with `IPluginLoader`.
- **Custom Initialization**: Implement initialization and execution logic with `IPlugin` or `BasePlugin`.

---

## Installation

Install the NuGet package using the `.NET CLI` or `Package Manager` in Visual Studio:

```bash
dotnet add package ScriptRunner.Plugins
```

---

## Usage

### Creating a Plugin

1. **Define a Plugin Class**:
   Annotate your class with `[PluginMetadata]` to provide details like the name, description, version, and system compatibility.

   ```csharp
   using ScriptRunner.Plugins.Attributes;
   using ScriptRunner.Plugins.Interfaces;

   [PluginMetadata(
       name: "ExamplePlugin",
       description: "A sample plugin for ScriptRunner",
       author: "Your Name",
       version: "1.0.0",
       pluginSystemVersion: "1.0.4",
       frameworkVersion: "net8.0")]
   public class ExamplePlugin : IPlugin
   {
       public string Name => "ExamplePlugin";

       public void Initialize(IDictionary<string, object> configuration)
       {
           Console.WriteLine("ExamplePlugin initialized.");
       }

       public void Execute()
       {
           Console.WriteLine("ExamplePlugin executed.");
       }
   }
   ```

2. **Register Services (Optional)**:
   If your plugin provides services, implement `IServicePlugin` and register them in the DI container.

   ```csharp
   using Microsoft.Extensions.DependencyInjection;

   public class ExamplePlugin : IServicePlugin
   {
       public string Name => "ExamplePlugin";

       public void Initialize(IDictionary<string, object> configuration)
       {
           Console.WriteLine("ExamplePlugin initialized.");
       }

       public void Execute()
       {
           Console.WriteLine("ExamplePlugin executed.");
       }

       public void RegisterServices(IServiceCollection services)
       {
           services.AddSingleton<IMyService, MyService>();
       }
   }
   ```

3. **Handle Dependencies**:
   If your plugin depends on other plugins, declare them using the `Dependencies` field in `PluginMetadata`.

   ```csharp
   [PluginMetadata(
       name: "ReportPlugin",
       description: "Generates reports.",
       author: "Your Name",
       version: "1.0.0",
       pluginSystemVersion: "1.0.4",
       dependencies: new[] { "DatabasePlugin", "LoggingPlugin" })]
   public class ReportPlugin : IServicePlugin
   {
       public string Name => "ReportPlugin";

       public void RegisterServices(IServiceCollection services)
       {
           services.AddSingleton<IReportService, ReportService>();
       }

       public void Initialize(IDictionary<string, object> configuration) { }
       public void Execute() { }
   }
   ```

---

## Simplify Development with Base Classes

The framework includes several base classes to simplify plugin development. These classes provide default implementations of common plugin interfaces, reducing boilerplate and allowing you to focus on your plugin's core functionality.

#### Example: `BasePlugin`

The `BasePlugin` class implements the `IPlugin` interface with default behavior for initialization (`Initialize`) and execution (`Execute`).

```csharp
using ScriptRunner.Plugins;

public class MyPlugin : BasePlugin
{
    public override string Name => "MyPlugin";

    public override void Initialize(IDictionary<string, object> configuration)
    {
        Console.WriteLine("MyPlugin initialized.");
    }

    public override void Execute()
    {
        Console.WriteLine("MyPlugin executed.");
    }
}
```

#### Comparison of Base Classes

| Base Class                   | Implements                  | Key Features                                                                 |
|------------------------------|-----------------------------|------------------------------------------------------------------------------|
| **`BasePlugin`**             | `IPlugin`                  | Basic plugin with `Initialize` and `Execute` methods.                       |
| **`BaseServicePlugin`**      | `IServicePlugin`           | Adds `RegisterServices` for DI integration.                                 |
| **`BaseAsyncPlugin`**        | `IAsyncPlugin`             | Async versions of `Initialize` and `Execute`.                               |
| **`BaseAsyncServicePlugin`** | `IAsyncServicePlugin`      | Async versions of `RegisterServices`, `Initialize`, and `Execute`.          |
| **`BaseLifecyclePlugin`**    | `ILifecyclePlugin`         | Adds lifecycle methods (`OnStart`, `OnStop`, `OnDispose`).                  |
| **`BaseGamePlugin`**         | `IGamePlugin`, `ILifecyclePlugin` | Adds frame-based methods (`Update` and `Render`) for game development. |

---

### Loading Plugins in ScriptRunner

1. **Discover and Validate Plugins**:
   Use `IPluginLoader` to discover and validate plugins dynamically.

   ```csharp
   using System.Reflection;
   using ScriptRunner.Plugins.Utilities;

   var pluginLoader = new PluginLoader(new PluginValidator());
   var plugins = pluginLoader.DiscoverAndValidatePlugins(Assembly.Load("YourPluginAssembly"));

   foreach (var pluginType in plugins)
   {
       Console.WriteLine($"Discovered plugin: {pluginType.Name}");
   }
   ```

2. **Use Plugins**:
   Instantiate and execute plugins after validation.

   ```csharp
   foreach (var pluginType in plugins)
   {
       var plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
       plugin.Initialize(new Dictionary<string, object>());
       plugin.Execute();
   }
   ```

---

## Extensibility

1. **Custom Validators**:
   Create a custom implementation of `IPluginValidator` to apply stricter validation rules.

   ```csharp
   public class CustomPluginValidator : IPluginValidator
   {
       public void Validate(Type pluginType)
       {
           // Custom validation logic
       }
   }
   ```

2. **Custom Loaders**:
   Implement `IPluginLoader` for specialized plugin discovery mechanisms.

   ```csharp
   public class CustomPluginLoader : IPluginLoader
   {
       public IEnumerable<Type> DiscoverAndValidatePlugins(Assembly assembly)
       {
           // Custom plugin discovery and validation logic
       }

       public IEnumerable<Type> DiscoverPlugins(Assembly assembly)
       {
           // Custom plugin discovery logic
       }
   }
   ```

---

## Contributing

Contributions are welcome! If you find a bug or have a feature request, open an issue or submit a pull request on [GitHub](https://github.com/petervdpas/ScriptRunner.Plugins).

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
