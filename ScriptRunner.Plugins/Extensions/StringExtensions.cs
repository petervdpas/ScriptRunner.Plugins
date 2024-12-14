using System;
using System.Linq;

namespace ScriptRunner.Plugins.Extensions;

/// <summary>
///     Provides extension methods for string operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Reverses the input string.
    /// </summary>
    /// <param name="input">The string to reverse.</param>
    /// <returns>The input string with characters in reverse order.</returns>
    public static string Reverse(this string input)
    {
        var charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    /// <summary>
    ///     Converts the input string to uppercase.
    /// </summary>
    public static string ToUpperCase(this string input)
    {
        return input.ToUpper();
    }

    /// <summary>
    ///     Converts the input string to lowercase.
    /// </summary>
    public static string ToLowerCase(this string input)
    {
        return input.ToLower();
    }
    
    /// <summary>
    /// Removes all white-space characters from the string.
    /// </summary>
    /// <param name="input">The string from which to remove white-space characters.</param>
    /// <returns>A new string with all white-space characters removed.</returns>
    public static string RemoveWhitespace(this string input)
    {
        return string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    }
    
    /// <summary>
    /// Checks if the string is a palindrome (reads the same backward as forward).
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <returns>True if the string is a palindrome... otherwise, false.</returns>
    public static bool IsPalindrome(this string input)
    {
        var reversed = input.Reverse();
        return string.Equals(input, reversed, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Converts the string to a title case (e.g., "hello world" becomes "Hello World").
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string in title case.</returns>
    public static string ToTitleCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input) 
            ? input 
            : string.Join(" ", input.Split(' ').Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
    }
    
    /// <summary>
    /// Converts the string to PascalCase (e.g., "hello world" becomes "HelloWorld").
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string in PascalCase.</returns>
    public static string ToPascalCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? input
            : string.Join("", input.Split(' ', '-', '_')
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
    }

    /// <summary>
    /// Converts the string to camelCase (e.g., "hello world" becomes "helloWorld").
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string in camelCase.</returns>
    public static string ToCamelCase(this string input)
    {
        var pascalCase = input.ToPascalCase();
        return string.IsNullOrWhiteSpace(pascalCase) 
            ? pascalCase 
            : char.ToLower(pascalCase[0]) + pascalCase[1..];
    }

    /// <summary>
    /// Converts the string to snake_case (e.g., "hello world" becomes "hello_world").
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string in snake_case.</returns>
    public static string ToSnakeCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? input
            : string.Join("_", input.Split(' ', '-', '_')
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => word.ToLower()));
    }

    /// <summary>
    /// Converts the string to a kebab-case (e.g., "hello world" becomes "hello-world").
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string in kebab-case.</returns>
    public static string ToKebabCase(this string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? input
            : string.Join("-", input.Split(' ', '-', '_')
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => word.ToLower()));
    }

}