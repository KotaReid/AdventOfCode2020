using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day23
    {
        public static void Run()
        {
            var input = "389125467";
            //var input = "614752839";

            Console.WriteLine($"Part 1: {Part1(input)}");
            Console.WriteLine($"Part 2: {Part2(input)}");
        }

        private static string Part1(string input)
        {
            var cups = input.Select(c => new Cup { Label = Int32.Parse(c.ToString()) }).ToList();

            for (var i = 0; i < cups.Count; i++)
                cups[i].NextCup = cups[(i + 1) % cups.Count];

            var cup = PlayGame(cups.ToDictionary(cup => cup.Label, cup => cup), moves: 100, maxValue: 9);

            var answer = string.Empty;
            for (var i = 0; i < cups.Count; i++)
            {
                answer += cup.NextCup.Label;
                cup = cup.NextCup;
            }

            return answer;
        }

        private static long Part2(string input)
        {
            var cups = input.Select(c => Int32.Parse(c.ToString()))
                            .Concat(Enumerable.Range(10, 1000000))
                            .Select(i => new Cup { Label = i })
                            .ToList();

            for (var i = 0; i < cups.Count; i++)
                cups[i].NextCup = cups[(i + 1) % cups.Count];

            var cup = PlayGame(cups.ToDictionary(cup => cup.Label, cup => cup), moves: 10000000, maxValue: 1000000);

            return (long)cup.NextCup.Label * cup.NextCup.NextCup.Label;
        }

        private static Cup PlayGame(Dictionary<int, Cup> cups, int moves, int maxValue)
        {
            var currentCup = cups.First().Value;

            for (var move = 1; move <= moves; move++)
            {
                var firstPickedUpCup = currentCup.NextCup;
                currentCup.NextCup = currentCup.NextCup.NextCup.NextCup;

                var destinationLabel = SetDestination(currentCup.Label - 1);

                while (destinationLabel == firstPickedUpCup.Label
                    || destinationLabel == firstPickedUpCup.NextCup.Label
                    || destinationLabel == firstPickedUpCup.NextCup.NextCup.Label)
                {
                    destinationLabel = SetDestination(destinationLabel - 1);
                }

                var destinationCup = cups[destinationLabel];
                firstPickedUpCup.NextCup.NextCup.NextCup = destinationCup.NextCup;
                destinationCup.NextCup = firstPickedUpCup;

                currentCup = currentCup.NextCup;
            }

            return cups[1];

            int SetDestination(int i) => i <= 0 ? maxValue : i;
        }

        public class Cup
        {
            public int Label { get; init; }
            public Cup NextCup { get; set; }
        }
    }
}