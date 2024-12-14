using System;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for converting a byte array to a <see cref="Guid" />.
/// </summary>
public static class ByteArrayExtensions
{
    /// <summary>
    ///     Converts a byte array to a <see cref="Guid" />.
    /// </summary>
    /// <param name="bytes">The byte array representing the GUID.</param>
    /// <returns>A <see cref="Guid" /> instance created from the byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="bytes" /> parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="bytes" /> parameter is not 16 bytes long.</exception>
    public static Guid ToGuid(this byte[] bytes)
    {
        return new Guid(bytes);
    }
}