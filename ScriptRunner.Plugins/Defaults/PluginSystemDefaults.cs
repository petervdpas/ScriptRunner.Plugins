namespace ScriptRunner.Plugins.Defaults;

/// <summary>
///     Provides default settings for the plugin system, including shared dependencies
///     and libraries to skip during validation.
/// </summary>
public static class PluginSystemDefaults
{
    /// <summary>
    ///     Gets the list of shared dependencies that are commonly required by plugins.
    ///     These dependencies are loaded globally and shared across all plugins to
    ///     avoid duplication and improve compatibility.
    /// </summary>
    /// <remarks>
    ///     Shared dependencies typically include libraries such as Microsoft.Extensions,
    ///     SQLite-related libraries, and other common .NET packages that plugins rely on.
    /// </remarks>
    public static readonly string[] DefaultSharedDependencies =
    [
        "Microsoft.CodeAnalysis.Common.dll",
        "Microsoft.CodeAnalysis.CSharp.dll",
        "Microsoft.CodeAnalysis.Analyzers.dll",
        "Microsoft.Data.Sqlite.dll",
        "Microsoft.Data.Sqlite.Core.dll",
        "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
        "Microsoft.Extensions.DependencyInjection.dll",
        "Microsoft.Extensions.Logging.Abstractions.dll",
        "Microsoft.Extensions.Logging.dll",
        "Microsoft.Extensions.Options.dll",
        "Microsoft.Extensions.Primitives.dll",
        "Microsoft.Build.Tasks.Git.dll",
        "Microsoft.SourceLink.GitHub.dll",
        "Microsoft.SourceLink.Common.dll",
        "Newtonsoft.Json.dll",
        "SQLitePCLRaw.bundle_e_sqlite3.dll",
        "SQLitePCLRaw.core.dll",
        "SQLitePCLRaw.lib.e_sqlite3.dll",
        "SQLitePCLRaw.provider.e_sqlite3.dll",
        "System.Collections.Immutable.dll",
        "System.Diagnostics.DiagnosticSource.dll",
        "System.Memory.dll",
        "System.Reflection.Metadata.dll",
        "System.Reflection.MetadataLoadContext.dll",
        "ScriptRunner.Plugins.dll"
    ];

    /// <summary>
    ///     Gets the list of libraries to skip during dependency validation.
    ///     These libraries are typically native or platform-specific files that do not
    ///     follow standard .NET dependency resolution rules.
    /// </summary>
    /// <remarks>
    ///     For example, the "e_sqlite3.dll" is a native SQLite library and is skipped
    ///     during dependency checks to prevent validation errors.
    /// </remarks>
    public static readonly string[] DefaultSkipLibraryChecks =
    [
        "e_sqlite3.dll"
    ];
}