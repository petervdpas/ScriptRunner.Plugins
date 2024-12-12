using System.Collections.Generic;

namespace ScriptRunner.Plugins.Interfaces;

/// <summary>
/// Interface for managing temporary, dynamic storage of key-value data with support for persistence.
/// </summary>
public interface ILocalStorage
{
    /// <summary>
    /// Adds or updates a value in the storage.
    /// </summary>
    /// <param name="key">The key associated with the data entry.</param>
    /// <param name="value">The value to store.</param>
    /// <remarks>
    /// If the key already exists, its value will be updated. Keys are case-sensitive.
    /// </remarks>
    void SetData(string key, object value);

    /// <summary>
    /// Retrieves a value from the storage.
    /// </summary>
    /// <typeparam name="T">The type of the data to retrieve.</typeparam>
    /// <param name="key">The key associated with the data entry.</param>
    /// <returns>
    /// The value associated with the key if found, otherwise the default value of <typeparamref name="T" />.
    /// </returns>
    /// <exception cref="System.ArgumentException">Thrown if the key is null or whitespace.</exception>
    T? GetData<T>(string key);

    /// <summary>
    /// Removes a key and its associated value from the storage.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <remarks>
    /// If the key does not exist, no action is taken.
    /// </remarks>
    /// <exception cref="System.ArgumentException">Thrown if the key is null or whitespace.</exception>
    void RemoveData(string key);

    /// <summary>
    /// Clears all entries from the storage.
    /// </summary>
    /// <remarks>
    /// Use this method with caution as it will remove all data without confirmation.
    /// </remarks>
    void ClearData();

    /// <summary>
    /// Lists all keys and their values stored in the storage.
    /// </summary>
    /// <returns>
    /// A string representing all keys and their associated values.
    /// Each entry is formatted as "Key: Value".
    /// </returns>
    string ListAllData();

    /// <summary>
    /// Retrieves the entire storage as a dictionary.
    /// </summary>
    /// <returns>
    /// An <see cref="IDictionary{TKey, TValue}" /> containing all key-value pairs in the storage.
    /// </returns>
    /// <remarks>
    /// The returned dictionary represents a snapshot of the storage.
    /// Modifying it does not affect the internal storage.
    /// </remarks>
    IDictionary<string, object> GetStorage();

    /// <summary>
    /// Saves the current storage data to a file in JSON format.
    /// </summary>
    /// <param name="filePath">The path to the file where the data will be saved.</param>
    /// <exception cref="System.ArgumentException">Thrown if the file path is null or whitespace.</exception>
    /// <exception cref="System.IO.IOException">Thrown if there is an error writing to the file.</exception>
    void SaveToFile(string filePath);

    /// <summary>
    /// Loads storage data from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the file from which the data will be loaded.</param>
    /// <exception cref="System.ArgumentException">Thrown if the file path is null or whitespace.</exception>
    /// <exception cref="System.IO.FileNotFoundException">Thrown if the specified file does not exist.</exception>
    /// <exception cref="System.IO.IOException">Thrown if there is an error reading the file.</exception>
    /// <exception cref="System.Text.Json.JsonException">Thrown if the file contents cannot be deserialized.</exception>
    void LoadFromFile(string filePath);
}