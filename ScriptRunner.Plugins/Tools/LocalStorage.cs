using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Provides a thread-safe local storage implementation using dynamic data for temporary storage.
///     Implements <see cref="ILocalStorage" />.
/// </summary>
public class LocalStorage : ILocalStorage
{
    // ExpandoObject to hold temporary data
    private readonly dynamic _tempData = new ExpandoObject();
    private readonly object _lock = new();

    /// <summary>
    ///     Adds or updates a value in the temporary storage dynamically.
    /// </summary>
    /// <param name="key">The key for the data entry.</param>
    /// <param name="value">The value to be stored.</param>
    public void SetData(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            tempDataDict[key] = value;
        }
    }

    /// <summary>
    ///     Retrieves a value from the temporary storage.
    /// </summary>
    /// <typeparam name="T">The type of the data to retrieve.</typeparam>
    /// <param name="key">The key of the data to retrieve.</param>
    /// <returns>The value associated with the key, or the default value of <typeparamref name="T" /> if the key is not found.</returns>
    public T? GetData<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict.TryGetValue(key, out var value) ? (T)value : default;
        }
    }

    /// <summary>
    ///     Removes a key and its associated value from the temporary storage.
    /// </summary>
    /// <param name="key">The key of the data to remove.</param>
    public void RemoveData(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            if (tempDataDict.ContainsKey(key))
                tempDataDict.Remove(key);
        }
    }

    /// <summary>
    ///     Clears all entries from the temporary storage.
    /// </summary>
    public void ClearData()
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            tempDataDict.Clear();
        }
    }

    /// <summary>
    ///     Lists all keys and their values stored in the temporary storage.
    /// </summary>
    /// <returns>A string representing all keys and their corresponding values.</returns>
    public string ListAllData()
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict.Aggregate(string.Empty, (current, kvp) => current + $"{kvp.Key}: {kvp.Value}\n");
        }
    }

    /// <summary>
    ///     Retrieves the entire temporary storage as a dictionary.
    /// </summary>
    /// <returns>An <see cref="IDictionary{TKey, TValue}" /> representing the storage.</returns>
    public IDictionary<string, object> GetStorage()
    {
        lock (_lock)
        {
            return new Dictionary<string, object>((IDictionary<string, object>)_tempData);
        }
    }

    /// <summary>
    ///     Saves the storage data to a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file where the data will be saved.</param>
    public void SaveToFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        lock (_lock)
        {
            var json = JsonSerializer.Serialize(GetStorage());
            File.WriteAllText(filePath, json);
        }
    }

    /// <summary>
    ///     Loads storage data from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file from which the data will be loaded.</param>
    public void LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file does not exist.", filePath);

        lock (_lock)
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (data == null) return;
            
            foreach (var kvp in data)
                SetData(kvp.Key, kvp.Value);
        }
    }
}
