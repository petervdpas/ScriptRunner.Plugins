using System.IO;
using System.Linq;
using ScriptRunner.Plugins.Tools;

namespace ScriptRunner.Plugins.Tests;

public class LocalStorageTests
{
    private readonly LocalStorage _localStorage;

    public LocalStorageTests()
    {
        _localStorage = new LocalStorage();
    }

    [Fact]
    public void SetData_AddsKeyValuePairs()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key2", 42);

        var retrievedValue1 = _localStorage.GetData<string>("Key1");
        var retrievedValue2 = _localStorage.GetData<int>("Key2");

        Assert.Equal("Value1", retrievedValue1);
        Assert.Equal(42, retrievedValue2);
    }

    [Fact]
    public void SetData_UpdatesExistingKeyValue()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key1", "UpdatedValue1");

        var retrievedValue = _localStorage.GetData<string>("Key1");
        Assert.Equal("UpdatedValue1", retrievedValue);
    }

    [Fact]
    public void GetData_ReturnsNullForNonExistentKey()
    {
        var value = _localStorage.GetData<string>("NonExistentKey");
        Assert.Null(value);
    }

    [Fact]
    public void RemoveData_RemovesKeyValue()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.RemoveData("Key1");

        var value = _localStorage.GetData<string>("Key1");
        Assert.Null(value);
    }

    [Fact]
    public void ClearData_RemovesAllEntries()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key2", 42);
        _localStorage.ClearData();

        Assert.Null(_localStorage.GetData<string>("Key1"));
    }

    [Fact]
    public void ListAllData_ReturnsAllKeyValuePairs()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key2", 42);

        var allData = _localStorage.ListAllData();
        Assert.Contains("Key1: Value1", allData);
        Assert.Contains("Key2: 42", allData);
    }

    [Fact]
    public void SaveAndLoadFromFile_PreservesData()
    {
        const string filePath = "test_storage.json";

        try
        {
            _localStorage.SetData("Key1", "Value1");
            _localStorage.SetData("Key2", true);

            _localStorage.SaveToFile(filePath);
            _localStorage.ClearData();

            Assert.Null(_localStorage.GetData<string>("Key1"));

            _localStorage.LoadFromFile(filePath);

            var value1 = _localStorage.GetData<string>("Key1");
            var value2 = _localStorage.GetData<bool>("Key2");

            Assert.Equal("Value1", value1);
            Assert.True(value2);
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void Events_AreTriggeredCorrectly()
    {
        var addedTriggered = false;
        var updatedTriggered = false;
        var removedTriggered = false;

        _localStorage.OnDataAdded += (key, value) => addedTriggered = key == "Key1" && (string)value! == "Value1";
        _localStorage.OnDataUpdated +=
            (key, value) => updatedTriggered = key == "Key1" && (string)value! == "UpdatedValue1";
        _localStorage.OnDataRemoved += key => removedTriggered = key == "Key1";

        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key1", "UpdatedValue1");
        _localStorage.RemoveData("Key1");

        Assert.True(addedTriggered);
        Assert.True(updatedTriggered);
        Assert.True(removedTriggered);
    }

    [Fact]
    public void GetKeysMatching_ReturnsMatchingKeys()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key2", 42);
        _localStorage.SetData("AnotherKey", "Test");

        var matchingKeys = _localStorage.GetKeysMatching("^Key[0-9]$").ToList();
        Assert.Contains("Key1", matchingKeys);
        Assert.Contains("Key2", matchingKeys);
        Assert.DoesNotContain("AnotherKey", matchingKeys);
    }

    [Fact]
    public void SearchKeysByValue_FindsKeysByValue()
    {
        _localStorage.SetData("Key1", "Value1");
        _localStorage.SetData("Key2", 42);
        _localStorage.SetData("Key3", "Value1");

        var keys = _localStorage.SearchKeysByValue("Value1").ToList();
        Assert.Contains("Key1", keys);
        Assert.Contains("Key3", keys);
        Assert.DoesNotContain("Key2", keys);
    }
}