using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
///     Interface for managing temporary, dynamic storage of key-value data with support for TTL and persistence.
/// </summary>
public interface ILocalStorage
{
    /// <summary>
    ///     Temporarily enables or disables the invocation of event handlers for data operations.
    /// </summary>
    /// <param name="suppress">
    ///     If <c>true</c>, event handlers for data operations (such as <see cref="OnDataAdded" />,
    ///     <see cref="OnDataUpdated" />, and <see cref="OnDataRemoved" />) will not be invoked.
    ///     If <c>false</c>, event handlers will be invoked as usual.
    /// </param>
    void SuppressEvents(bool suppress);

    /// <summary>
    ///     Triggered when data is added to the storage.
    /// </summary>
    event Action<string, object?>? OnDataAdded;

    /// <summary>
    ///     Triggered when data in the storage is updated.
    /// </summary>
    event Action<string, object?>? OnDataUpdated;

    /// <summary>
    ///     Triggered when data is removed from the storage.
    /// </summary>
    event Action<string>? OnDataRemoved;

    /// <summary>
    ///     Adds or updates a value in the storage with optional TTL (time-to-live).
    /// </summary>
    /// <param name="key">The key associated with the data entry.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="ttl">
    ///     Optional TTL duration for the entry.
    ///     The entry will expire and be removed after this duration.
    /// </param>
    /// <exception cref="ArgumentException">Thrown if the key is null or whitespace.</exception>
    void SetData(string key, object? value, TimeSpan? ttl = null);

    /// <summary>
    ///     Retrieves a value from the storage.
    /// </summary>
    /// <typeparam name="T">The type of the data to retrieve.</typeparam>
    /// <param name="key">The key associated with the data entry.</param>
    /// <returns>
    ///     The value associated with the key if found, otherwise the default value of <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the key is null or whitespace.</exception>
    T? GetData<T>(string key);

    /// <summary>
    ///     Removes a key and its value from the storage.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <exception cref="ArgumentException">Thrown if the key is null or whitespace.</exception>
    void RemoveData(string key);

    /// <summary>
    ///     Clears all entries from the storage.
    /// </summary>
    /// <remarks>
    ///     Use this method with caution as it will remove all data without confirmation.
    /// </remarks>
    void ClearData();

    /// <summary>
    ///     Lists all keys and their values stored in the storage.
    /// </summary>
    /// <returns>
    ///     A string representing all keys and their associated values. Each entry is formatted as "Key: Value".
    /// </returns>
    string ListAllData();

    /// <summary>
    ///     Retrieves all keys matching a regex pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match keys.</param>
    /// <returns>A collection of keys that match the specified pattern.</returns>
    /// <exception cref="ArgumentException">Thrown if the pattern is null or whitespace.</exception>
    IEnumerable<string> GetKeysMatching(string pattern);

    /// <summary>
    ///     Searches keys by their associated values.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>A collection of keys that have the specified value.</returns>
    IEnumerable<string> SearchKeysByValue(object value);

    /// <summary>
    ///     Retrieves the entire storage as a dictionary.
    /// </summary>
    /// <returns>
    ///     An <see cref="IDictionary{TKey, TValue}" /> containing all key-value pairs in the storage.
    /// </returns>
    /// <remarks>
    ///     The returned dictionary represents a snapshot of the storage. Modifying it does not affect the internal storage.
    /// </remarks>
    IDictionary<string, object> GetStorage();

    /// <summary>
    ///     Saves the current storage data to a file in JSON format.
    /// </summary>
    /// <param name="filePath">The path to the file where the data will be saved.</param>
    /// <exception cref="ArgumentException">Thrown if the file path is null or whitespace.</exception>
    /// <exception cref="IOException">Thrown if there is an error writing to the file.</exception>
    void SaveToFile(string filePath);

    /// <summary>
    ///     Loads storage data from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file from which the data will be loaded.</param>
    /// <exception cref="ArgumentException">Thrown if the file path is null or whitespace.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist.</exception>
    /// <exception cref="IOException">Thrown if there is an error reading the file.</exception>
    /// <exception cref="JsonException">Thrown if the file contents cannot be deserialized.</exception>
    void LoadFromFile(string filePath);
}