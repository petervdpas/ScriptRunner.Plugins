using System;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for converting a <see cref="Guid" /> to a byte array.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    ///     Converts a <see cref="Guid" /> to its byte array representation.
    /// </summary>
    /// <param name="guid">The <see cref="Guid" /> to convert.</param>
    /// <returns>A byte array representing the GUID.</returns>
    public static byte[] ToByteArray(this Guid guid)
    {
        return guid.ToByteArray();
    }
}