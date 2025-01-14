using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ScriptRunner.Plugins.Interfaces;
using ScriptRunner.Plugins.Utilities;

namespace ScriptRunner.Plugins.Tools;

/// <inheritdoc />
public class LocalStorage : ILocalStorage
{
    private readonly Dictionary<string, DateTime> _expirationData = new();
    private readonly object _lock = new();
    private readonly Dictionary<string, object> _tempData = new();
    private bool _suppressEvents;

    /// <inheritdoc />
    public void SuppressEvents(bool suppress)
    {
        _suppressEvents = suppress;
    }

    /// <inheritdoc />
    public event Action<string, object?>? OnDataAdded = (key, value) => { };

    /// <inheritdoc />
    public event Action<string, object?>? OnDataUpdated = (key, value) => { };

    /// <inheritdoc />
    public event Action<string>? OnDataRemoved = key => { };

    /// <inheritdoc />
    public void SetData(string key, object? value, TimeSpan? ttl = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        lock (_lock)
        {
            var serializedValue = value as string ?? SerializationHelper.Serialize(value);

            if (!_tempData.TryAdd(key, serializedValue))
            {
                _tempData[key] = serializedValue;

                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);
                else
                    _expirationData.Remove(key);

                RaiseEvent(OnDataUpdated, key, serializedValue);
            }
            else
            {
                if (ttl.HasValue)
                    _expirationData[key] = DateTime.UtcNow.Add(ttl.Value);

                RaiseEvent(OnDataAdded, key, serializedValue);
            }
        }
    }

    /// <inheritdoc />
    public T? GetData<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            if (!_expirationData.TryGetValue(key, out var expiration) || DateTime.UtcNow <= expiration)
            {
                if (!_tempData.TryGetValue(key, out var value)) return default;

                return value switch
                {
                    string strValue when typeof(T) == typeof(string) => (T)(object)strValue, // Direct return for string
                    string strValue => SerializationHelper.Deserialize<T>(strValue), // Deserialize for other types
                    T castValue => castValue, // Directly cast if value matches T
                    _ => default // Fallback
                };
            }

            _tempData.Remove(key);
            _expirationData.Remove(key);
            OnDataRemoved?.Invoke(key);

            return default;
        }
    }

    /// <inheritdoc />
    public void RemoveData(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        lock (_lock)
        {
            if (!_tempData.Remove(key)) return;

            _expirationData.Remove(key);
            OnDataRemoved?.Invoke(key);
        }
    }

    /// <inheritdoc />
    public void ClearData()
    {
        lock (_lock)
        {
            var keys = _tempData.Keys.ToList();
            _tempData.Clear();
            _expirationData.Clear();

            foreach (var key in keys) OnDataRemoved?.Invoke(key);
        }
    }

    /// <inheritdoc />
    public string ListAllData()
    {
        lock (_lock)
        {
            return string.Join("\n", _tempData.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> GetKeysMatching(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Pattern cannot be null or whitespace.", nameof(pattern));

        lock (_lock)
        {
            var regex = new Regex(pattern);
            return _tempData.Keys.Where(key => regex.IsMatch(key)).ToList();
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> SearchKeysByValue(object value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        lock (_lock)
        {
            return _tempData
                .Where(kvp => kvp.Value.Equals(value))
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }

    /// <inheritdoc />
    public IDictionary<string, object> GetStorage()
    {
        lock (_lock)
        {
            return _tempData;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

            if (data == null)
            {
                Console.WriteLine("No data found in the file. Loading skipped.");
                return;
            }

            foreach (var (key, value) in data)
                if (value is JsonElement element && element.ValueKind == JsonValueKind.String)
                    // Handle string values to strip unnecessary quotes
                    SetData(key, element.GetString());
                else
                    SetData(key, value);
        }
    }

    private void RaiseEvent(Action<string, object?>? eventHandler, string key, object? value)
    {
        if (!_suppressEvents) eventHandler?.Invoke(key, value);
    }
}