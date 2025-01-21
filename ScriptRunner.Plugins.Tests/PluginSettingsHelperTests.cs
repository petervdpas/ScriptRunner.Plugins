using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Models;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.Tests;

public class PluginSettingsHelperTests
{
    private readonly Mock<ILocalStorage> _mockLocalStorage;

    public PluginSettingsHelperTests()
    {
        _mockLocalStorage = new Mock<ILocalStorage>();
        PluginSettingsHelper.InitializeLocalStorage(_mockLocalStorage.Object);
    }

    [Fact]
    public void InitializeLocalStorage_ThrowsArgumentNullException_WhenLocalStorageIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => PluginSettingsHelper.InitializeLocalStorage(null));
    }

    [Fact]
    public void FetchLocalStorage_ReturnsLocalStorage_WhenInitialized()
    {
        var localStorage = PluginSettingsHelper.FetchLocalStorage();
        Assert.NotNull(localStorage);
    }

    [Fact]
    public void FetchLocalStorage_ThrowsInvalidOperationException_WhenNotInitialized()
    {
        // Ensure the local storage is not initialized
        var field = typeof(PluginSettingsHelper)
            .GetField("_localStorage", BindingFlags.Static | BindingFlags.NonPublic);

        // Clear the field to simulate uninitialized state
        field?.SetValue(null, null);

        Assert.Throws<InvalidOperationException>(PluginSettingsHelper.FetchLocalStorage);
    }

    [Fact]
    public void StoreSettings_ThrowsInvalidOperationException_WhenLocalStorageNotInitialized()
    {
        // Ensure _localStorage is null explicitly
        typeof(PluginSettingsHelper)
            .GetField("_localStorage", BindingFlags.NonPublic | BindingFlags.Static)?
            .SetValue(null, null);

        var settings = new List<PluginSettingDefinition>
        {
            new() { Key = "TestKey", Value = "TestValue" }
        };

        // Assert that the exception is thrown
        var exception = Assert.Throws<InvalidOperationException>(() => PluginSettingsHelper.StoreSettings(settings));
        Assert.Equal("LocalStorage has not been initialized.", exception.Message);
    }

    [Fact]
    public void StoreSettings_ThrowsArgumentNullException_WhenSettingsAreNull()
    {
        Assert.Throws<ArgumentNullException>(() => PluginSettingsHelper.StoreSettings(null!));
    }

    [Fact]
    public void StoreSettings_SerializesAndStoresEachSetting()
    {
        var settings = new List<PluginSettingDefinition>
        {
            new() { Key = "Key1", Value = "Value1" },
            new() { Key = "Key2", Value = true }
        };

        PluginSettingsHelper.StoreSettings(settings);

        // Verify expectations based on the actual serialization logic
        _mockLocalStorage.Verify(
            x => x.SetData("Key1", "Value1", null), Times.Once); // String remains unquoted
        _mockLocalStorage.Verify(
            x => x.SetData("Key2", "true", null), Times.Once); // Boolean is serialized as a string
    }

    [Fact]
    public void RetrieveSetting_ReturnsDefault_WhenKeyNotFound()
    {
        _mockLocalStorage.Setup(x => x.GetData<string>("NonExistingKey"))
            .Returns((string)null!);

        var result = PluginSettingsHelper.RetrieveSetting<string>("NonExistingKey");

        Assert.Null(result);
    }

    [Fact]
    public void RetrieveSetting_DeserializesAndReturnsStoredValue()
    {
        // Mock GetData to return a plain serialized string
        _mockLocalStorage.Setup(x => x.GetData<string>("TestKey"))
            .Returns("TestValue"); // Already a plain string, no JSON wrapping

        var result = PluginSettingsHelper.RetrieveSetting<string>("TestKey");

        Assert.Equal("TestValue", result);
    }

    [Fact]
    public void RetrieveSetting_ReturnsDefault_WhenDeserializationFails()
    {
        _mockLocalStorage.Setup(x => x.GetData<string>("InvalidKey"))
            .Returns("Invalid JSON");

        var result = PluginSettingsHelper.RetrieveSetting<int>("InvalidKey");

        Assert.Equal(0, result); // Default for int
    }

    [Fact]
    public void DisplayStoredSettings_PrintsAllStoredSettings()
    {
        var storedData = new Dictionary<string, object>
        {
            { "Key1", "\"Value1\"" },
            { "Key2", "true" },
            { "Key3", "Value2" },
            { "Key4", true },
            { "SecretKey", "SensitiveValue" }
        };

        _mockLocalStorage.Setup(x => x.GetStorage())
            .Returns(storedData);

        // Mock the schema to indicate "SecretKey" is a secret
        var schema = new List<PluginSettingDefinition>
        {
            new() { Key = "Key1", Value = "Value1", IsSecret = false },
            new() { Key = "Key2", Value = true, IsSecret = false },
            new() { Key = "Key3", Value = "Value2", IsSecret = false },
            new() { Key = "Key4", Value = true, IsSecret = false },
            new() { Key = "SecretKey", Value = "SensitiveValue", IsSecret = true }
        };
        PluginSettingsHelper.StoreSettings(schema);

        using var consoleOutput = new ConsoleOutput();
        PluginSettingsHelper.DisplayStoredSettings();

        var output = consoleOutput.GetOutput();

        // Verify output
        Assert.Contains("- Key: Key1, Value: \"Value1\"", output);
        Assert.Contains("- Key: Key2, Value: true", output);
        Assert.Contains("- Key: Key3, Value: Value2", output);
        Assert.Contains("- Key: Key4, Value: True", output); // Match the expected format
        Assert.Contains("- Key: SecretKey, Value: *****", output); // Secret key is masked
    }

    [Fact]
    public void DisplayStoredSettings_PrintsMessage_WhenNoSettingsFound()
    {
        _mockLocalStorage.Setup(x => x.GetStorage())
            .Returns(new Dictionary<string, object>());

        using var consoleOutput = new ConsoleOutput();
        PluginSettingsHelper.DisplayStoredSettings();

        var output = consoleOutput.GetOutput();
        Assert.Contains("No settings found in local storage.", output);
    }
    
    [Fact]
    public void IsSecretSetting_ThrowsException_WhenSchemaNotInitialized()
    {
        var field = typeof(PluginSettingsHelper)
            .GetField("_schema", BindingFlags.Static | BindingFlags.NonPublic);

        // Ensure the schema is not initialized
        field?.SetValue(null, null);

        var isSecretMethod = typeof(PluginSettingsHelper)
            .GetMethod("IsSecretSetting", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(isSecretMethod);

        var exception = Assert.Throws<TargetInvocationException>(() =>
            isSecretMethod.Invoke(null, ["AnyKey"])
        );

        Assert.IsType<InvalidOperationException>(exception.InnerException);
        Assert.Equal("Settings schema has not been initialized.", exception.InnerException.Message);
    }
}