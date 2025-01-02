---
Title: Exploring Math Extensions  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, MathExtensions, ScriptRunner, Mathematics]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true  
---

# Recipe: Exploring Math Extensions

## Goal

Demonstrate the usage of the **MathExtensions** library to perform mathematical operations in ScriptRunner scripts.

## Steps

### 1. Write a Script

Create a ScriptRunner script that uses the **MathExtensions** library.
The script will showcase various mathematical operations such as
addition, subtraction, multiplication, division, power calculation, clamping values,
absolute values, modulus, square root, and factorials.

### 2. Run the Script

Execute the script in ScriptRunner to see the results of each operation.

---

## Example Script

Below is an example script that demonstrates the functionality of the **MathExtensions** library:

```csharp
/*
{
    "TaskCategory": "Mathematical",
    "TaskName": "MathExtension",
    "TaskDetail": "A demo script for the Math Extension"
}
*/

int a = 10;
int b = 5;

// Add
var sum = a.Add(b);
Dump($"Sum: {sum}");

// Subtract
var difference = a.Subtract(b);
Dump($"Difference: {difference}");

// Multiply
var product = a.Multiply(b);
Dump($"Product: {product}");

// Divide
var quotient = a.Divide(b);
Dump($"Quotient: {quotient}");

// Power
double baseNumber = 2;
double exponent = 3;
var powerResult = baseNumber.Power(exponent);
Dump($"Power: {baseNumber} ^ {exponent} = {powerResult}");

// Clamp
int value = 15;
int min = 0;
int max = 10;
var clampedValue = value.Clamp(min, max);
Dump($"Clamp: {value} clamped to range {min}-{max} = {clampedValue}");

// Absolute Value
int negativeValue = -25;
var absoluteValue = negativeValue.Abs();
Dump($"Absolute Value: Abs({negativeValue}) = {absoluteValue}");

// Square Root
double sqrtValue = 16;
var squareRoot = sqrtValue.Sqrt();
Dump($"Square Root: Sqrt({sqrtValue}) = {squareRoot}");

// Modulus
int dividend = 10;
int divisor = 3;
var modulus = dividend.Mod(divisor);
Dump($"Modulus: {dividend} % {divisor} = {modulus}");

// Factorial
int factorialInput = 5;
var factorialResult = factorialInput.Factorial();
Dump($"Factorial: {factorialInput}! = {factorialResult}");

return "MathExtension demo completed.";
```  

---

## Explanation of the Script

1. **Basic Arithmetic**: Demonstrates addition, subtraction, multiplication, and division using **Add**, **Subtract**, *
   *Multiply**, and **Divide** methods.
2. **Power Calculation**: Uses the **Power** method to raise a number to a specific exponent.
3. **Clamping Values**: Utilizes the **Clamp** method to ensure a value lies within a specified range.
4. **Absolute Value**: Converts a negative number to a positive one using the **Abs** method.
5. **Square Root**: Calculates the square root of a number using the **Sqrt** method.
6. **Modulus**: Finds the remainder of division with the **Mod** method.
7. **Factorials**: Computes the factorial of a number using the **Factorial** method.

---

## What You Can Do Next

- **Extend the Script**: Add more operations or integrate the script into larger workflows.
- **Dynamic Input**: Modify the script to accept user input for calculations via a ScriptRunner dialog.
- **Explore More**: Use these extensions in other mathematical scenarios, such as financial or scientific calculations.

---

This recipe provides an overview of the MathExtensions library and its utility in creating powerful
mathematical scripts in ScriptRunner. Try it out and extend it further to fit your needs!
