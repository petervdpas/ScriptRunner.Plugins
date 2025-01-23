/*
{
    "TaskCategory": "Extensions",
    "TaskName": "DateTimeExtensions",
    "TaskDetail": "A demo script showcasing DateTime extension methods",
    "RequiredPlugins": []
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
