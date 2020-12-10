using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day10
    {
        public static void Run()
        {
            var max = 0;
            var adapterJoltages = Utils.ReadFromFile("Day10.txt")
                .Select(s =>
                {
                    var num = int.Parse(s);
                    if (num > max)
                        max = num;
                    return num;
                }).ToList();

            adapterJoltages.Add(max + 3);

            Console.WriteLine($"Part 1: {Part1(adapterJoltages)}");
            Console.WriteLine($"Part 2: {Part2(adapterJoltages)}");
        }

        private static int Part1(List<int> adapterJoltages)
        {
            adapterJoltages.Sort();

            var currentJoltage = 0;
            var count1Delta = 0;
            var count3Delta = 0;

            foreach (var adapterJoltage in adapterJoltages)
            {
                var delta = adapterJoltage - currentJoltage;
                switch (delta)
                {
                    case 1:
                        count1Delta++;
                        break;
                    case 2:
                        break;
                    case 3:
                        count3Delta++;
                        break;
                    default:
                        throw new ArgumentException($"Adapter cannot connect. Delta {delta}");
                }
                currentJoltage = adapterJoltage;
            }

            return count1Delta * count3Delta;
        }

        private static long Part2(List<int> adapterJoltages)
        {
            adapterJoltages.Add(0);
            adapterJoltages.Sort();

            var counts = adapterJoltages.ToDictionary(j => j, _ => 1L);
            for (var i = adapterJoltages.Count - 1; i >= 0; i--)
            {
                var joltage = adapterJoltages[i];

                var connectingPathCount = adapterJoltages.Skip(i + 1)
                    .TakeWhile(otherJoltage => otherJoltage <= joltage + 3)
                    .Select(otherJoltage => counts[otherJoltage])
                    .Sum();

                if (connectingPathCount != 0)
                    counts[joltage] = connectingPathCount;
            }

            return counts[0];
        }
    }
}