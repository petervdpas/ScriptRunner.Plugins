﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ScriptRunner.Plugins.Interfaces;

namespace ScriptRunner.Plugins.Tools;

/// <summary>
///     Provides a thread-safe local storage implementation with support for TTL and event hooks.
///     Implements <see cref="ILocalStorage" />.
/// </summary>
public class LocalStorage : ILocalStorage
{
    private readonly Dictionary<string, DateTime> _expirationData = new();
    private readonly object _lock = new();
    private readonly dynamic _tempData = new ExpandoObject();

    /// <summary>
    ///     Triggered when a new key-value pair is added to the storage.
    /// </summary>
    public event Action<string, object> OnDataAdded = null!;

    /// <summary>
    ///     Triggered when an existing key-value pair in the storage is updated.
    /// </summary>
    public event Action<string, object> OnDataUpdated = null!;

    /// <summary>
    ///     Triggered when a key-value pair is removed from the storage.
    /// </summary>
    public event Action<string> OnDataRemoved = null!;

    /// <summary>
    ///     Adds or updates a value in the storage with optional TTL.
    /// </summary>
    public void SetData(string key, object value, TimeSpan? ttl = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;

            if (!tempDataDict.TryAdd(key, value))
            {
                tempDataDict[key] = value;
                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);
                else
                    _expirationData.Remove(key);

                OnDataUpdated(key, value);
            }
            else
            {
                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);

                OnDataAdded(key, value);
            }
        }
    }

    /// <summary>
    ///     Retrieves a value from the storage.
    /// </summary>
    public T? GetData<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;

            // Check TTL expiration
            if (!_expirationData.TryGetValue(key, out var expiration) || DateTime.UtcNow <= expiration)
                return tempDataDict.TryGetValue(key, out var value) ? (T)value : default;

            tempDataDict.Remove(key);
            _expirationData.Remove(key);
            OnDataRemoved(key);
            return default;
        }
    }

    /// <summary>
    ///     Removes a key and its value from the storage.
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
            OnDataRemoved(key);
        }
    }

    /// <summary>
    ///     Clears all entries from the storage.
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
    ///     Lists all keys and their values.
    /// </summary>
    public string ListAllData()
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict.Aggregate(string.Empty, (current, kvp) => current + $"{kvp.Key}: {kvp.Value}\n");
        }
    }

    /// <summary>
    ///     Retrieves all keys matching a regex pattern.
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
    ///     Searches keys by their associated values.
    /// </summary>
    public IEnumerable<string> SearchKeysByValue(object value)
    {
        lock (_lock)
        {
            var tempDataDict = (IDictionary<string, object>)_tempData;
            return tempDataDict.Where(kvp => kvp.Value.Equals(value)).Select(kvp => kvp.Key).ToList();
        }
    }

    /// <summary>
    ///     Retrieves the entire storage as a dictionary.
    /// </summary>
    public IDictionary<string, object> GetStorage()
    {
        lock (_lock)
        {
            return new Dictionary<string, object>((IDictionary<string, object>)_tempData);
        }
    }

    /// <summary>
    ///     Saves the storage data to a file.
    /// </summary>
    public void SaveToFile(string filePath, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        lock (_lock)
        {
            var json = JsonSerializer.Serialize(GetStorage(), options ?? new JsonSerializerOptions());
            File.WriteAllText(filePath, json);
        }
    }

    /// <summary>
    ///     Loads storage data from a file.
    /// </summary>
    public void LoadFromFile(string filePath, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file does not exist.", filePath);

        lock (_lock)
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json,
                options ?? new JsonSerializerOptions());
            if (data == null) return;

            foreach (var kvp in data) SetData(kvp.Key, kvp.Value);
        }
    }
}