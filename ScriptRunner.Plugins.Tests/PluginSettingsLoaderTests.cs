using System;
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
            new RawPluginSettingDefinition { Key = "ApiKey", Type = "string", DefaultValue = "default-key", IsSecret = true },
            new RawPluginSettingDefinition { Key = "Timeout", Type = "int", DefaultValue = 30, IsSecret = false }
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(rawSettings));

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            // Assert ApiKey
            Assert.Equal("ApiKey", result[0].Key);
            Assert.Equal("string", result[0].Type);
            Assert.Equal("default-key", result[0].Value);
            Assert.True(result[0].IsSecret);

            // Assert Timeout
            Assert.Equal("Timeout", result[1].Key);
            Assert.Equal("int", result[1].Type);

            // Handle type mismatch by converting
            Assert.Equal(30, Convert.ToInt32(result[1].Value)); // Convert to int
            Assert.False(result[1].IsSecret);
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
    public void MergeSettings_RetainsIsSecretProperty()
    {
        var schema = new List<PluginSettingDefinition>
        {
            new() { Key = "ApiKey", Type = "string", Value = "default-key", IsSecret = true },
            new() { Key = "Timeout", Type = "int", Value = 30, IsSecret = false }
        };

        var userValues = new List<PluginSettingDefinition>
        {
            new() { Key = "ApiKey", Type = "string", Value = "user-key", IsSecret = true },
            new() { Key = "Timeout", Type = "int", Value = 60, IsSecret = false }
        };

        var result = PluginSettingsLoader.MergeSettings(schema, userValues).ToList();

        Assert.Equal(2, result.Count);

        // Assert ApiKey
        var apiKeySetting = result.First(s => s.Key == "ApiKey");
        Assert.Equal("user-key", apiKeySetting.Value);
        Assert.True(apiKeySetting.IsSecret);

        // Assert Timeout
        var timeoutSetting = result.First(s => s.Key == "Timeout");
        Assert.Equal(60, timeoutSetting.Value);
        Assert.False(timeoutSetting.IsSecret);
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
    public void MergeSettings_UsesSchemaDefaults_ForIsSecret()
    {
        var schema = new List<PluginSettingDefinition>
        {
            new() { Key = "ApiKey", Type = "string", Value = "default-key", IsSecret = true },
            new() { Key = "Timeout", Type = "int", Value = 30, IsSecret = false }
        };

        var result = PluginSettingsLoader.MergeSettings(schema, null).ToList();

        Assert.Equal(2, result.Count);

        // Assert ApiKey
        var apiKeySetting = result.First(s => s.Key == "ApiKey");
        Assert.Equal("default-key", apiKeySetting.Value);
        Assert.True(apiKeySetting.IsSecret);

        // Assert Timeout
        var timeoutSetting = result.First(s => s.Key == "Timeout");
        Assert.Equal(30, timeoutSetting.Value);
        Assert.False(timeoutSetting.IsSecret);
    }

    [Fact]
    public void LoadSettings_ValidatesTypeCompatibility()
    {
        const string filePath = "plugin.settings.json";
        var rawSettings = new[]
        {
            new RawPluginSettingDefinition { Key = "Key1", Type = "int", DefaultValue = 10 },
            new RawPluginSettingDefinition { Key = "Key2", Type = "bool", DefaultValue = "not-a-bool" }
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(rawSettings));

        try
        {
            var result = PluginSettingsLoader.LoadSettings(".", true);

            Assert.NotNull(result);
            Assert.Equal(2, result.Length);

            // Validate Key1
            var key1Setting = result.First(s => s.Key == "Key1");
            Assert.Equal("Key1", key1Setting.Key);
            Assert.Equal("int", key1Setting.Type);

            // Check type and value with conversion
            Assert.IsType<double>(key1Setting.Value); // JSON numbers default to double
            Assert.Equal(10, Convert.ToInt32(key1Setting.Value)); // Convert for int comparison

            // Validate Key2
            var key2Setting = result.First(s => s.Key == "Key2");
            Assert.Equal("Key2", key2Setting.Key);
            Assert.Equal("bool", key2Setting.Type);
            Assert.Null(key2Setting.Value); // Invalid value should result in null
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}