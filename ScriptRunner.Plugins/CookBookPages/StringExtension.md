---
Title: Exploring String Extensions  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, StringExtensions, ScriptRunner, Strings]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true  
---

# Recipe: Exploring String Extensions

## Goal

Demonstrate the usage of the **StringExtensions** library to perform various operations on strings in ScriptRunner
scripts.

---

## Steps

### 1. Write a Script

Create a ScriptRunner script that uses the **StringExtensions** library.  
The script will showcase operations such as reversing a string, case conversions, removing whitespace,  
checking for palindromes, and converting to various string cases like PascalCase, camelCase, snake_case, and kebab-case.

### 2. Run the Script

Execute the script in ScriptRunner to observe the results of each operation.

---

## Example Script

Below is an example script that demonstrates the functionality of the **StringExtensions** library:

```csharp
/*
{
    "TaskCategory": "StringManipulation",
    "TaskName": "StringExtensionsDemo",
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
```  

---

## Explanation of the Script

1. **Reverse a String**:
    - Demonstrates reversing the characters in a string using **Reverse**.
    - Example: **"Hello World"** becomes **"dlroW olleH"**.

2. **Case Conversions**:
    - Converts a string to uppercase using **ToUpperCase**.
    - Converts a string to lowercase using **ToLowerCase**.

3. **Remove Whitespace**:
    - Removes all whitespace from a string using **RemoveWhitespace**.
    - Example: **"Hello World"** becomes **"HelloWorld"**.

4. **Check for Palindromes**:
    - Check if a string reads the same backward as forward using **IsPalindrome**.
    - Example: **"Racecar"** returns **true**.

5. **Convert to Title Case**:
    - Capitalizes the first letter of each word using **ToTitleCase**.
    - Example: **"hello world example"** becomes **"Hello World Example"**.

6. **String Case Conversions**:
    - Converts strings to:
        - **PascalCase** using **ToPascalCase**: **"hello world"** becomes **"HelloWorld"**.
        - **camelCase** using **ToCamelCase**: **"hello world"** becomes **"helloWorld"**.
        - **snake_case** using **ToSnakeCase**: **"hello world"** becomes **"hello_world"**.
        - **kebab-case** using **ToKebabCase**: **"hello world"** becomes **"hello-world"**.

---

## What You Can Do Next

1. **Dynamic Inputs**:
    - Modify the script to accept user-provided strings via a ScriptRunner dialog.

2. **Combine Operations**:
    - Use the extension methods to create complex string manipulations, such as generating unique identifiers.

3. **Explore Additional Use Cases**:
    - Use these methods in scenarios such as formatting user input, generating API-friendly keys, or preparing text for
      database storage.

---

This recipe demonstrates how to leverage the **StringExtensions** library to simplify and enhance string
operations in ScriptRunner. Experiment with the methods to fit your use cases!
