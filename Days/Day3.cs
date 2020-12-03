using System;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day3
    {
        public static void Run()
        {
            var grid = Utils.ReadFromFile("Day3.txt")
                            .Select(s => s.Select(c => c == '#').ToArray())
                            .ToArray();

            Console.WriteLine($"Part 1: {Part1(grid)}");
            Console.WriteLine($"Part 2: {Part2(grid)}");
        }

        private static int Part1(bool[][] grid) => grid.GetTrees(1, 3);

        private static long Part2(bool[][] grid) => (long)grid.GetTrees(1, 1)
                                                        * grid.GetTrees(1, 3)
                                                        * grid.GetTrees(1, 5)
                                                        * grid.GetTrees(1, 7)
                                                        * grid.GetTrees(2, 1);

        private static int GetTrees(this bool[][] grid, int rise, int run)
        {
            var numberOfTrees = 0;
            var x = 0;

            for (var i = rise; i < grid.Length; i += rise)
            {
                x += run;

                var row = grid[i];

                if (row[x % grid[0].Length])
                    numberOfTrees++;
            }

            return numberOfTrees;
        }
    }
}