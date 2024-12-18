using System.Linq;
using System.Reflection;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
/// Provides extension methods for working with assemblies and their metadata.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Retrieves the public key token of an <see cref="AssemblyName"/> as a hexadecimal string.
    /// </summary>
    /// <param name="assemblyName">The <see cref="AssemblyName"/> to retrieve the token from.</param>
    /// <returns>The public key token as a hexadecimal string, or <c>null</c> if no token is present.</returns>
    public static string? GetPublicKeyTokenString(this AssemblyName assemblyName)
    {
        var tokenBytes = assemblyName.GetPublicKeyToken();
        return tokenBytes != null
            ? string.Join(string.Empty, tokenBytes.Select(b => b.ToString("x2")))
            : null;
    }
}