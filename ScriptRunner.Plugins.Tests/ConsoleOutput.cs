using System;
using System.IO;

namespace ScriptRunner.Plugins.Tests;

/// <summary>
///     Helper class to capture console output during tests.
/// </summary>
public class ConsoleOutput : IDisposable
{
    private readonly TextWriter _originalOutput;
    private readonly StringWriter _stringWriter;

    public ConsoleOutput()
    {
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_stringWriter);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }

    public string GetOutput()
    {
        return _stringWriter.ToString();
    }
}