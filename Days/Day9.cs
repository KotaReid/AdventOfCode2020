using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day9
    {
        public static void Run()
        {
            var numbers = Utils.ReadFromFile("Day9.txt").Select(s => long.Parse(s)).ToArray();

            var part1 = Part1(numbers);
            Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Part 2: {Part2(numbers, part1)}");
        }

        private static long Part1(long[] numbers)
        {
            var preambleLength = 25;

            for (var i = preambleLength; i < numbers.Count(); i++)
            {
                var pairFound = false;
                var seen = new HashSet<long>();

                for (var j = i - preambleLength; j < i + 1; j++)
                {
                    var diff = numbers[i + 1] - numbers[j];

                    if (seen.Contains(diff))
                    {
                        pairFound = true;
                        break;
                    }

                    seen.Add(numbers[j]);
                }

                if (!pairFound)
                    return numbers[i + 1];
            }

            return -1;
        }

        private static long Part2(long[] numbers, long answer)
        {
            var range = new Queue<long>();
            var sum = 0L;
            for (var i = 0; i < numbers.Count(); i++)
            {
                sum += numbers[i];
                range.Enqueue(numbers[i]);

                while (sum > answer)
                    sum -= range.Dequeue();

                if (range.Count >= 2 && sum == answer)
                    return range.Min() + range.Max();
            }

            return -1;
        }
    }
}