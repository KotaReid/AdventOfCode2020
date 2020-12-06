using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day6
    {
        public static void Run()
        {
            var declarationForms = Utils.ReadFileAsString("Day6.txt")
                .Split(new [] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Part 1: {Part1(declarationForms)}");
            Console.WriteLine($"Part 2: {Part2(declarationForms)}");
        }

        private static int Part1(IEnumerable<string> declarationForms)
        {
            var groups = declarationForms.Select(s => s.Replace(Environment.NewLine, string.Empty));

            var sum = 0;

            foreach (var group in groups)
            {
                var seen = new HashSet<char>();

                foreach (var c in group)
                {
                    if (seen.Contains(c))
                        continue;

                    seen.Add(c);
                }

                sum += seen.Count();
            }
            return sum;
        }

        private static int Part2(IEnumerable<string> declarationForms)
        {
            var groups = declarationForms.Select(s => s.Split(Environment.NewLine));

            var sum = 0;
            foreach (var group in groups)
            {
                var counts = new int[26];

                foreach (var person in group)
                {
                    foreach (var c in person)
                    {
                        counts[c - 'a']++;
                    }
                }

                sum += counts.Count(c => c == group.Count());
            }

            return sum;
        }
    }
}