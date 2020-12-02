using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day1
    {
        public static void Run()
        {
            var numbers = Utils.ReadFromFile("Day1.txt").Select(s => Int32.Parse(s)).ToList();

            Console.WriteLine($"Part 1: {Part1(numbers)}");
            Console.WriteLine($"Part 2: {Part2(numbers)}");
        }

        private static int Part1(List<int> numbers)
        {
            for (var i = 0; i < numbers.Count - 2; i++)
                for (var j = i + 1; j < numbers.Count; j++)
                {
                    var number1 = numbers[i];
                    var number2 = numbers[j];

                    if (number1 + number2 == 2020)
                    {
                        Console.WriteLine($"Numbers: {number1} & {number2}");
                        return number1 * number2;
                    }
                }

            return -1;
        }

        private static int Part2(List<int> numbers)
        {
            for (var i = 0; i < numbers.Count - 2; i++)
                for (var j = i + 1; j < numbers.Count; j++)
                    for (var k = j + 1; k < numbers.Count; k++)
                    {
                        var number1 = numbers[i];
                        var number2 = numbers[j];
                        var number3 = numbers[k];

                        if (number1 + number2 + number3 == 2020)
                        {
                            Console.WriteLine($"Numbers: {number1} & {number2} & {number3}");
                            return number1 * number2 * number3;
                        }
                    }

            return -1;
        }
    }
}