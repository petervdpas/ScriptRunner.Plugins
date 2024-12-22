/*
{
    "TaskCategory": "Extensions",
    "TaskName": "StringExtension",
    "TaskDetail": "A demo script showcasing string extension methods"
}
*/

var sampleString = "Hello World";

// Reverse
var reversed = sampleString.Reverse();
Dump($"Reversed: {reversed}");

// To Upper Case
var upperCase = sampleString.ToUpperCase();
Dump($"Upper Case: {upperCase}");

// To Lower Case
var lowerCase = sampleString.ToLowerCase();
Dump($"Lower Case: {lowerCase}");

// Remove Whitespace
var noWhitespace = sampleString.RemoveWhitespace();
Dump($"No Whitespace: {noWhitespace}");

// Is Palindrome
var palindromeString = "Racecar";
var isPalindrome = palindromeString.IsPalindrome();
Dump($"Is Palindrome ('{palindromeString}'): {isPalindrome}");

// To Title Case
var titleCase = "hello world example".ToTitleCase();
Dump($"Title Case: {titleCase}");

// To Pascal Case
var pascalCase = "hello world example".ToPascalCase();
Dump($"Pascal Case: {pascalCase}");

// To Camel Case
var camelCase = "hello world example".ToCamelCase();
Dump($"Camel Case: {camelCase}");

// To Snake Case
var snakeCase = "hello world example".ToSnakeCase();
Dump($"Snake Case: {snakeCase}");

// To Kebab Case
var kebabCase = "hello world example".ToKebabCase();
Dump($"Kebab Case: {kebabCase}");

return "StringExtensions demo completed.";
