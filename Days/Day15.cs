using System;
using System.Collections.Generic;

namespace AdventOfCode2020.Days
{
    public static class Day15
    {
        public static void Run()
        {
            var input = new List<int> { 1, 2, 16, 19, 18, 0 };
            Console.WriteLine($"Part 1: {MemoryGame(input, 2020)}");
            Console.WriteLine($"Part 2: {MemoryGame(input, 30000000)}");
        }

        private static long MemoryGame(List<int> input, int targetStep)
        {
            var spokenNumbers = new Dictionary<int, int>();

            var turn = 1;
            input.ForEach(i => spokenNumbers[i] = turn++);

            var nextSpokenNumber = 0;
            for (; turn < targetStep; turn++)
                nextSpokenNumber = spokenNumbers.NextSpokenNumber(nextSpokenNumber, turn);

            return nextSpokenNumber;
        }

        private static int NextSpokenNumber(this Dictionary<int, int> spokenNumbers, int number, int turn)
        {
            if (spokenNumbers.ContainsKey(number))
            {
                var nextNumber = turn - spokenNumbers[number];
                spokenNumbers[number] = turn;
                return nextNumber;
            }

            spokenNumbers[number] = turn;
            return 0;
        }
    }
}