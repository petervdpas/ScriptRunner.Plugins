using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
/// Provides a thread-safe local storage implementation with support for TTL and event hooks.
/// Implements <see cref="ILocalStorage" />.
/// </summary>
public class LocalStorage : ILocalStorage
{
    private readonly Dictionary<string, DateTime> _expirationData = new();
    private readonly object _lock = new();
    private readonly dynamic _tempData = new ExpandoObject();
    private bool _suppressEvents;

    /// <summary>
    /// Temporarily enables or disables the invocation of event handlers for data operations.
    /// </summary>
    /// <param name="suppress">
    /// If <c>true</c>, event handlers for data operations (such as <see cref="OnDataAdded"/>,
    /// <see cref="OnDataUpdated"/>, and <see cref="OnDataRemoved"/>) will not be invoked.
    /// If <c>false</c>, event handlers will be invoked as usual.
    /// </param>
    public void SuppressEvents(bool suppress) => _suppressEvents = suppress;

    /// <summary>
    /// Triggered when a new key-value pair is added to the storage.
    /// </summary>
    public event Action<string, object?>? OnDataAdded = (key, value) => { };

    /// <summary>
    /// Triggered when an existing key-value pair in the storage is updated.
    /// </summary>
    public event Action<string, object?>? OnDataUpdated = (key, value) => { };

    /// <summary>
    /// Triggered when a key-value pair is removed from the storage.
    /// </summary>
    public event Action<string>? OnDataRemoved = key => { };

    /// <summary>
    /// Adds or updates a value in the storage with optional TTL.
    /// </summary>
    public void SetData(string key, object? value, TimeSpan? ttl = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object?>)_tempData;

            value = value switch
            {
                string str when str.StartsWith('{') || str.StartsWith('[') => str, // Keep serialized JSON strings as-is
                _ => SerializationHelper.Serialize(value) // Serialize non-JSON objects
            };

            if (!tempDataDict.TryAdd(key, value))
            {
                tempDataDict[key] = value;
                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);
                else
                    _expirationData.Remove(key);

                RaiseEvent(OnDataUpdated, key, value);
            }
            else
            {
                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);

                RaiseEvent(OnDataAdded, key, value);
            }
        }
    }

    /// <summary>
    /// Retrieves a value from the storage.
    /// </summary>
    public T? GetData<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;

            if (!_expirationData.TryGetValue(key, out var expiration) || DateTime.UtcNow <= expiration)
            {
                return !tempDataDict.TryGetValue(key, out var value) 
                    ? default 
                    : SerializationHelper.Deserialize<T>(value.ToString() ?? string.Empty);
            }

            tempDataDict.Remove(key);
            _expirationData.Remove(key);
            OnDataRemoved?.Invoke(key);

            return default;
        }
    }

    /// <summary>
    /// Removes a key and its value from the storage.
    /// </summary>
    public void RemoveData(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            if (!tempDataDict.Remove(key)) return;

            _expirationData.Remove(key);
            OnDataRemoved?.Invoke(key);
        }
    }

    /// <summary>
    /// Clears all entries from the storage.
    /// </summary>
    public void ClearData()
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            var keys = tempDataDict.Keys.ToList();
            tempDataDict.Clear();
            _expirationData.Clear();

            foreach (var key in keys) OnDataRemoved?.Invoke(key);
        }
    }

    /// <summary>
    /// Lists all keys and their values.
    /// </summary>
    public string ListAllData()
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return string.Join("\n", tempDataDict.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
    }

    /// <summary>
    /// Retrieves all keys matching a regex pattern.
    /// </summary>
    public IEnumerable<string> GetKeysMatching(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Pattern cannot be null or whitespace.", nameof(pattern));

        lock (_lock)
        {
            var regex = new Regex(pattern);
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict.Keys.Where(key => regex.IsMatch(key)).ToList();
        }
    }

    /// <summary>
    /// Searches for keys in the storage that have the specified value.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A collection of keys that have the specified value.</returns>
    public IEnumerable<string> SearchKeysByValue(object value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict
                .Where(kvp => kvp.Value.Equals(value))
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }

    /// <summary>
    /// Retrieves the entire storage as a dictionary.
    /// </summary>
    public IDictionary<string, object> GetStorage()
    {
        lock (_lock)
        {
            return new Dictionary<string, object>((IDictionary<string, object>)_tempData);
        }
    }

    /// <summary>
    /// Saves the storage data to a file.
    /// </summary>
    public void SaveToFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        lock (_lock)
        {
            var json = SerializationHelper.Serialize(GetStorage());
            File.WriteAllText(filePath, json);
        }
    }

    /// <summary>
    /// Loads storage data from a file.
    /// </summary>
    public void LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file does not exist.", filePath);

        lock (_lock)
        {
            var json = File.ReadAllText(filePath);
            var data = SerializationHelper.Deserialize<Dictionary<string, object>>(json);

            if (data == null) return;

            foreach (var (key, value) in data)
            {
                SetData(key, value);
            }
        }
    }

    private void RaiseEvent(Action<string, object?>? eventHandler, string key, object? value)
    {
        if (!_suppressEvents) eventHandler?.Invoke(key, value);
    }
}