﻿using System;
using System.Collections.Generic;
using AdventOfCode2020.Days;

var Days = new Dictionary<int, Action> {
    {1, () => Day1.Run()},
    {2, () => Day2.Run()},
    {3, () => Day3.Run()},
    {4, () => Day4.Run()},
    {5, () => Day5.Run()},
    {6, () => Day6.Run()},
    {7, () => Day7.Run()},
    {8, () => Day8.Run()},
    {9, () => Day9.Run()},
    {10, () => Day10.Run()},
    {11, () => Day11.Run()},
    {12, () => Day12.Run()},
    {13, () => Day13.Run()},
    {14, () => Day14.Run()},
    {15, () => Day15.Run()},
    {16, () => Day16.Run()},
    {17, () => Day17.Run()},
    {18, () => Day18.Run()},
    {19, () => Day19.Run()},
    {20, () => Day20.Run()},
    {21, () => Day21.Run()},
    {22, () => Day22.Run()},
    {23, () => Day23.Run()},
    {24, () => Day24.Run()},
    {25, () => Day25.Run()}
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
