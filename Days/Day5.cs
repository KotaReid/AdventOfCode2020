using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day5
    {
        public static void Run()
        {
            var boardingPasses = Utils.ReadFromFile("Day5.txt").Select(s => s.Trim());

            var boardingPassIds = boardingPasses.GetIds();

            Console.WriteLine($"Part 1: {boardingPassIds.Max()}");
            Console.WriteLine($"Part 2: {Part2(boardingPassIds)}");
        }

        private static int Part2(IEnumerable<int> boardingPassIds)
        {
            var minId = 9; //Start of row 1
            var maxId = boardingPassIds.Max();

            var emptySeatIds = Enumerable.Range(minId, maxId).Except(boardingPassIds).ToList();

            for (var i = 1; i < emptySeatIds.Count - 1; i++)
            {
                if (emptySeatIds[i] != emptySeatIds[i - 1] + 1 && emptySeatIds[i] != emptySeatIds[i + 1] - 1)
                    return emptySeatIds[i];
            }

            return -1;
        }

        private static IEnumerable<int> GetIds(this IEnumerable<string> boardingPasses)
        {
            return boardingPasses.Select(boardingPass =>
            {
                var row = GetRow(boardingPass.Take(7));
                var col = GetCol(boardingPass.TakeLast(3));
                return row * 8 + col;
            });
        }

        private static int GetRow(IEnumerable<char> s) => FindPosition(s, 127, 'F', 'B');

        private static int GetCol(IEnumerable<char> s) => FindPosition(s, 7, 'L', 'R');

        private static int FindPosition(IEnumerable<char> s, int max, char bottom, char top)
        {
            var min = 0;

            foreach (var c in s)
            {
                double mid = (max - min) / 2.0 + min;
                if (c == bottom)
                    max = (int) Math.Floor(mid);
                else if (c == top)
                    min = (int) Math.Ceiling(mid);
            }

            return min;
        }
    }
}