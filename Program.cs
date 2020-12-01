using System;
using System.Collections.Generic;
using AdventOfCode.Days;

var Days = new Dictionary<int, Action> {
    {1, () => Day1.Run()}
};

while (true)
{
    Console.Write("Enter Number of Day to Execute [q to quit]: ");
    var input = Console.ReadLine()?.Trim();

    if (input is null or "q" or "Q")
        break;

    if (Int32.TryParse(input.Trim(), out var day) && Days.ContainsKey(day))
        Days[day].Invoke();
    else
        Console.WriteLine("Invalid input.");
}