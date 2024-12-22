/*
{
    "TaskCategory": "Extensions",
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