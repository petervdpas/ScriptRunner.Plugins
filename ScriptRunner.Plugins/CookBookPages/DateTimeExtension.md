---
Title: Exploring DateTime Extensions  
Category: Cookbook  
Author: Peter van de Pas  
keywords: [CookBook, DateTimeExtensions, ScriptRunner, DateTime]  
table-use-row-colors: true  
table-row-color: "D3D3D3"  
toc: true  
toc-title: Table of Content  
toc-own-page: true  
---

# Recipe: Exploring DateTime Extensions  

## Goal  

Demonstrate the usage of the **DateTimeExtensions** library to perform various operations 
on dates and times in ScriptRunner scripts.  

---

## Steps  

### 1. Write a Script  

Create a ScriptRunner script that uses the **DateTimeExtensions** library. The script will showcase operations 
such as formatting dates, calculating Unix timestamps, finding start and end of the day, adding working days, 
checking if a date is a weekend or weekday, and more.  

### 2. Run the Script  

Execute the script in ScriptRunner to observe the results of each operation.  

---

## Example Script  

Below is an example script that demonstrates the functionality of the **DateTimeExtensions** library:  

```csharp
/*
{
    "TaskCategory": "DateTime",
    "TaskName": "DateTimeExtensionsDemo",
    "TaskDetail": "A demo script showcasing DateTime extension methods"
}
*/

var now = DateTime.Now;

// Format Date
var formattedDate = now.FormatDate("dddd, MMMM dd, yyyy");
Dump($"Formatted Date: {formattedDate}");

// Get Year, Month, Day
var year = now.GetYear();
var month = now.GetMonth();
var day = now.GetDay();
Dump($"Year: {year}, Month: {month}, Day: {day}");

// Unix Timestamp
var unixTimestamp = now.ToUnixTimestamp();
var fromUnixTimestamp = unixTimestamp.FromUnixTimestamp();
Dump($"Unix Timestamp: {unixTimestamp}");
Dump($"Converted Back from Unix Timestamp: {fromUnixTimestamp}");

// Start and End of Day
var startOfDay = now.StartOfDay();
var endOfDay = now.EndOfDay();
Dump($"Start of Day: {startOfDay}");
Dump($"End of Day: {endOfDay}");

// Add Working Days
var workingDaysLater = now.AddWorkingDays(5);
Dump($"Date 5 Working Days Later: {workingDaysLater}");

// Is Weekend or Weekday
var isWeekend = now.IsWeekend();
var isWeekday = now.IsWeekday();
Dump($"Is Weekend: {isWeekend}");
Dump($"Is Weekday: {isWeekday}");

// Days in Month
var totalDaysInMonth = now.TotalDaysInMonth();
var daysRemainingInMonth = now.DaysRemainingInMonth();
Dump($"Total Days in Month: {totalDaysInMonth}");
Dump($"Days Remaining in Month: {daysRemainingInMonth}");

// Time Difference
var futureDate = now.AddDays(10);
var timeDifference = now.GetTimeDifference(futureDate);
Dump($"Time Difference (now to 10 days later): {timeDifference}");

return "DateTimeExtensions demo completed.";
```  

---

## Explanation of the Script

1. **Formatting Dates**:
    - Formats the current date using **FormatDate**. You can customize the format string as needed.

2. **Extracting Components**:
    - Extracts year, month, and day using **GetYear**, **GetMonth**, and **GetDay**.

3. **Unix Timestamps**:
    - Convert the current date to a Unix timestamp with **ToUnixTimestamp** and back to a **DateTime** with **FromUnixTimestamp**.

4. **Start and End of Day**:
    - Calculate the start and end times of the current day using **StartOfDay** and **EndOfDay**.

5. **Adding Working Days**:
    - Calculate the date after adding 5 working days, skipping weekends, using **AddWorkingDays**.

6. **Checking Weekends and Weekdays**:
    - Determines if the current date is a weekend or weekday using **IsWeekend** and **IsWeekday**.

7. **Month Calculations**:
    - Retrieves the total days in the current month with **TotalDaysInMonth** and the remaining days in the month with **DaysRemainingInMonth**.

8. **Time Difference**:
    - Computes the time difference between the current date and a future date using **GetTimeDifference**.

---

## What You Can Do Next

1. **Extend the Script**:
    - Add more use cases, such as working with time zones or calculating business days.

2. **Dynamic Inputs**:
    - Modify the script to accept dynamic inputs for date calculations via ScriptRunner dialogs.

3. **Integrate with Other Extensions**:
    - Combine **DateTimeExtensions** with other extensions like **MathExtensions** for advanced scenarios.

---

This recipe demonstrates how to leverage the **DateTimeExtensions** library to simplify 
and enhance date-time operations in ScriptRunner. Start using these powerful extensions 
to handle all your date and time needs effortlessly!
