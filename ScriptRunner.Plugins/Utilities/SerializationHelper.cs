using System;
using System.Text.Json;

namespace ScriptRunner.Plugins.Utilities;

/// <summary>
/// Provides utility methods for serializing and deserializing objects, with support for handling JSON.
/// </summary>
public static class SerializationHelper
{
    private static readonly JsonSerializerOptions? JsonOptions = new()
    {
        WriteIndented = false
    };
    
    /// <summary>
    /// Serializes an object into a JSON string. If the object is already a valid JSON string, it is returned as-is.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize(object value)
    {
        if (value is string strValue)
        {
            // Return as-is for valid JSON strings or raw strings
            return strValue;
        }

        // Serialize non-string objects
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    /// <summary>
    /// Deserializes a JSON string into an object of type <typeparamref name="T"/>.
    /// If deserialization fails and the target type is <see cref="string"/>, the raw string is returned.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize into.</typeparam>
    /// <param name="value">The JSON string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T"/> if successful, otherwise a raw string if the type is <see cref="string"/>.</returns>
    /// <exception cref="JsonException">Thrown when deserialization fails for non-string types.</exception>
    public static T? Deserialize<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        try
        {
            // Directly return if T is string
            if (typeof(T) == typeof(string))
                return (T)(object)value;

            // Deserialize for other types
            return JsonSerializer.Deserialize<T>(value, JsonOptions);
        }
        catch
        {
            // Fallback: Return default for failed deserialization
            return default;
        }
    }

    /// <summary>
    /// Deserializes a <see cref="JsonElement"/> into a .NET object, determining the type dynamically.
    /// </summary>
    /// <param name="element">The JSON element to deserialize.</param>
    /// <returns>
    /// A .NET object representation of the JSON element. 
    /// Returns a <see cref="string"/>, <see cref="int"/>, <see cref="double"/>, <see cref="bool"/>, or the element's raw string representation, depending on the JSON type.
    /// </returns>
    public static object? Deserialize(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt32(out var intValue) ? intValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object or JsonValueKind.Array => element.GetRawText(), // Preserve nested JSON as string
            _ => element.ToString() // For null or unknown cases
        };
    }
}
