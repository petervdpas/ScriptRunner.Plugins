using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for JSON operations using Newtonsoft.Json.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    ///     Converts an object to a formatted JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A formatted JSON string representation of the object.</returns>
    public static string ToJson<T>(this T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object to convert cannot be null.");

        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }

    /// <summary>
    ///     Converts a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The type to which the JSON string will be deserialized.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type T created from the JSON string.</returns>
    public static T? FromJson<T>(this string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentNullException(nameof(json), "JSON string to convert cannot be null or empty.");

        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    ///     Reformats a JSON string to a pretty-printed structure.
    /// </summary>
    /// <param name="json">The JSON string to reformat.</param>
    /// <returns>A pretty-printed JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the input JSON string is null or empty.</exception>
    /// <exception cref="JsonReaderException">Thrown if the input JSON string is invalid.</exception>
    public static string ReformatJson(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json), "JSON string cannot be null or empty.");

        try
        {
            // Parse the JSON string into a JToken
            var parsedJson = JToken.Parse(json);

            // Return the formatted JSON string
            return parsedJson.ToString(Formatting.Indented);
        }
        catch (JsonReaderException ex)
        {
            throw new JsonReaderException("Invalid JSON string provided for reformatting.", ex);
        }
    }
}