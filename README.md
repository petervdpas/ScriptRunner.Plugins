# ScriptRunner.Plugins

[![NuGet](https://img.shields.io/nuget/v/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ScriptRunner.Plugins.svg)](https://www.nuget.org/packages/ScriptRunner.Plugins)
[![License](https://img.shields.io/github/license/petervdpas/ScriptRunner.Plugins.svg)](https://opensource.org/licenses/MIT)

`ScriptRunner.Plugins` provides interfaces, attributes, utilities, and constants for building plugins compatible with
the `ScriptRunner` framework. It ensures seamless plugin development with dynamic loading, validation, and integration.

---

## Features

- **Plugin Metadata**: Annotate plugins with `[PluginMetadata]` for easy discovery and validation.
- **Version & Framework Constants**: Ensure compatibility with `PluginSystemConstants`.
- **Dynamic Plugin Management**: Discover, validate, and load plugins using `IPluginLoader` and `DependencyLoader`.
- **Database Utilities**: Use `IDatabase` for executing SQL queries and `SqlGenerator` for dynamic SQL generation.
- **Local Storage**: Store plugin data with `ILocalStorage` for runtime use and persistence.
- **Simplified Development**: Use base classes like `BasePlugin` and `BaseServicePlugin` to reduce boilerplate code.
- **Plugin Template**: Quickly scaffold a new plugin project using the provided template.

---

## Installation

Install via NuGet:

```bash
dotnet add package ScriptRunner.Plugins
```

---

## Getting Started

### Scaffold a New Plugin Project

The `ScriptRunner.Plugins` package includes a template to help you quickly set up a plugin project. Use the `dotnet new`
command:

```bash
dotnet new srplugin -n MyNewPlugin
```

This creates a project with:

- Example plugin class (`Plugin.cs`) using `[PluginMetadata]`.
- Pre-configured `PluginSystemConstants` for versioning.
- Example service registration with `IServicePlugin`.

---

## Quick Start

### Create a Plugin

1. Define your plugin class and use `[PluginMetadata]` for metadata:

   ```csharp
   using ScriptRunner.Plugins.Attributes;
   using ScriptRunner.Plugins.Utilities;

   [PluginMetadata(
       name: "SamplePlugin",
       description: "A simple plugin example.",
       author: "Your Name",
       version: "1.0.0",
       pluginSystemVersion: PluginSystemConstants.CurrentPluginSystemVersion,
       frameworkVersion: PluginSystemConstants.CurrentFrameworkVersion)]
   public class SamplePlugin : IPlugin
   {
       public void Initialize(IDictionary<string, object> configuration) => Console.WriteLine("Initialized.");
       public void Execute() => Console.WriteLine("Executed.");
   }
   ```

2. Register services if needed:

   ```csharp
   public class SampleServicePlugin : IServicePlugin
   {
       public void RegisterServices(IServiceCollection services)
       {
           services.AddSingleton<IMyService, MyService>();
       }
   }
   ```

---

## Utilities

### **Versioning with Constants**

Use `PluginSystemConstants` to ensure host and plugin version compatibility:

```csharp
public static class PluginSystemConstants
{
    public const string CurrentPluginSystemVersion = "1.2.0";
    public const string CurrentFrameworkVersion = ".NET 8.0";
}
```

### **Dynamic Dependency Management**

Load dependencies dynamically with `DependencyLoader`:

```csharp
DependencyLoader.SetSkipLibraries(new[] { "sqlite3.dll" });
DependencyLoader.LoadDependencies("path/to/dependencies", new ConcurrentDictionary<string, bool>(), logger);
```

### **Database Utilities**

- Use `IDatabase` for database interaction.
- Use `SqlGenerator` for generating queries dynamically:

```csharp
var generator = new SqlGenerator();
generator.SetType(typeof(MyEntity));
generator.SetTableName("MyTable");
var query = generator.GenerateSelectQuery();
```

### **Local Storage**

Use `ILocalStorage` for temporary data storage and persistence:

```csharp
var storage = new LocalStorage();
storage.SetData("key", "value");
storage.SaveToFile("storage.json");
storage.LoadFromFile("storage.json");
```

---

## Simplified Development

Use base classes like `BasePlugin` or `BaseAsyncServicePlugin` to minimize repetitive code.

Example with `BasePlugin`:

```csharp
public class MyPlugin : BasePlugin
{
    public override void Initialize(IDictionary<string, object> configuration) => Console.WriteLine("Initialized.");
    public override void Execute() => Console.WriteLine("Executed.");
}
```

---

## Contributing

Contributions are welcome! Submit issues or PRs on [GitHub](https://github.com/petervdpas/ScriptRunner.Plugins).

---

## License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
