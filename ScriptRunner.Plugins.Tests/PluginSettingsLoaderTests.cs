using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.Tests;

public class PluginSettingsLoaderTests
{
    [Fact]
    public void LoadSettings_ReturnsEmptyArray_WhenFileDoesNotExist()
    {
        const string pluginPath = "NonExistentPath";
        var result = PluginSettingsLoader.LoadSettings(pluginPath, true);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void LoadSettings_ReturnsEmptyArray_WhenFileIsEmpty()
    {
        const string filePath = "plugin.settings.json";
        File.WriteAllText(filePath, string.Empty);

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void LoadSettings_ParsesValidJsonFile()
    {
        const string filePath = "plugin.settings.json";
        var rawSettings = new[]
        {
            new RawPluginSettingDefinition { Key = "Key1", Type = "string", DefaultValue = "Value1" },
            new RawPluginSettingDefinition { Key = "Key2", Type = "bool", DefaultValue = true }
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(rawSettings));

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            // Assert Key1
            Assert.Equal("Key1", result[0].Key);
            Assert.Equal("string", result[0].Type);
            Assert.Equal("Value1", result[0].Value);

            // Assert Key2
            Assert.Equal("Key2", result[1].Key);
            Assert.Equal("bool", result[1].Type);
            Assert.IsType<bool>(result[1].Value); // Explicit type check
            Assert.True((bool)result[1].Value!); // Correctly cast and compare
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void LoadSettings_HandlesInvalidJsonGracefully()
    {
        const string filePath = "plugin.settings.json";
        File.WriteAllText(filePath, "{ InvalidJson: true }");

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void MergeSettings_OverridesDefaultValuesWithUserValues()
    {
        var schema = new List<PluginSettingDefinition>
        {
            new() { Key = "Key1", Type = "string", Value = "DefaultValue1" },
            new() { Key = "Key2", Type = "bool", Value = false }
        };

        var userValues = new List<PluginSettingDefinition>
        {
            new() { Key = "Key1", Type = "string", Value = "UserValue1" },
            new() { Key = "Key2", Type = "bool", Value = true }
        };

        var result = PluginSettingsLoader.MergeSettings(schema, userValues).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("UserValue1", result.First(s => s.Key == "Key1").Value);
        Assert.Equal(true, result.First(s => s.Key == "Key2").Value);
    }

    [Fact]
    public void MergeSettings_UsesSchemaDefaults_WhenNoUserValueProvided()
    {
        var schema = new List<PluginSettingDefinition>
        {
            new() { Key = "Key1", Type = "string", Value = "DefaultValue1" },
            new() { Key = "Key2", Type = "bool", Value = false }
        };

        var result = PluginSettingsLoader.MergeSettings(schema, null).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("DefaultValue1", result.First(s => s.Key == "Key1").Value);
        Assert.Equal(false, result.First(s => s.Key == "Key2").Value);
    }

    [Fact]
    public void LoadSettings_ValidatesTypeCompatibility()
    {
        const string filePath = "plugin.settings.json";
        var rawSettings = new[]
        {
            new RawPluginSettingDefinition { Key = "Key1", Type = "string", DefaultValue = 123 },
            new RawPluginSettingDefinition { Key = "Key2", Type = "bool", DefaultValue = "not-a-bool" }
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(rawSettings));

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
            Assert.Null(result.First(s => s.Key == "Key1").Value); // 123 is incompatible with string
            Assert.Null(result.First(s => s.Key == "Key2").Value); // "not-a-bool" is incompatible with bool
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}