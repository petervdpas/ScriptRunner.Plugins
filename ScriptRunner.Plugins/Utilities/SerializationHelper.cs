using System.Text.Json;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
///     Provides utility methods for serializing and deserializing objects, with support for handling JSON.
/// </summary>
public static class SerializationHelper
{
    private static readonly JsonSerializerOptions? JsonOptions = new()
    {
        WriteIndented = false
    };

    /// <summary>
    ///     Serializes an object into a JSON string. If the object is already a valid JSON string, it is returned as-is.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize(object value)
    {
        if (value is string strValue)
            // Return strings as-is without additional quotes
            return strValue;

        // Serialize non-string objects into JSON
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    /// <summary>
    ///     Deserializes a JSON string into an object of type <typeparamref name="T" />.
    ///     If deserialization fails and the target type is <see cref="string" />, the raw string is returned.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>
    ///     An object of type <typeparamref name="T" /> if successful, otherwise a raw string if the type is
    ///     <see cref="string" />.
    /// </returns>
    /// <exception cref="JsonException">Thrown when deserialization fails for non-string types.</exception>
    public static T? Deserialize<T>(string value)
    {
        if (typeof(T) == typeof(string))
            // If T is string, just return the raw string without processing
            return (T)(object)value;

        try
        {
            return JsonSerializer.Deserialize<T>(value, JsonOptions);
        }
        catch
        {
            // Fallback for invalid JSON
            return default;
        }
    }
}