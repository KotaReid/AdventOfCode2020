using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day16
    {
        public static void Run()
        {
            var (rules, myTicket, nearbyTickets) = ProcessInput(Utils.ReadFileAsString("Day16.txt"));

            var (validTickets, answer) = Part1(rules, nearbyTickets);

            Console.WriteLine($"Part 1: {answer}");
            Console.WriteLine($"Part 2: {Part2(rules, myTicket, validTickets)}");
        }

        private static (IEnumerable<int[]> ValidTickets, int Answer) Part1(IEnumerable<Rule> rules, IEnumerable<int[]> nearbyTickets)
        {
            var invalidFields = new List<int>();
            var validTickets = new List<int[]>();

            foreach (var nearbyTicket in nearbyTickets)
            {
                var validTicket = true;

                foreach (var field in nearbyTicket)
                {
                    if (rules.All(rule => !rule.IsValid(field)))
                    {
                        validTicket = false;
                        invalidFields.Add(field);
                    }
                }

                if (validTicket)
                    validTickets.Add(nearbyTicket);
            }

            return (validTickets, invalidFields.Sum());
        }

        private static long Part2(IEnumerable<Rule> rules, int[] myTicket, IEnumerable<int[]> validTickets)
        {
            var fields = new Dictionary<Rule, int>();

            while (fields.Count() < rules.Count())
            {
                for (var i = 0; i < myTicket.Length; i++)
                {
                    if (fields.Values.Contains(i))
                        continue;

                    var rulesToCheck = rules.Where(rule => !fields.ContainsKey(rule)).ToList();

                    foreach (var field in validTickets.Select(t => t[i]))
                    {
                        rulesToCheck = rulesToCheck.Where(rule => rule.IsValid(field)).ToList();

                        if (rulesToCheck.Count() == 1)
                        {
                            fields[rulesToCheck.Single()] = i;
                            break;
                        }
                    }
                }
            }

            var indexes = fields.Where(field => field.Key.Name.StartsWith("departure"))
                                .Select(field => field.Value);

            return indexes.Select(i => (long)myTicket[i]).Aggregate((a, b) => a * b);
        }

        private static Regex _ruleRegex = new Regex(@"^(?<name>.*): (?<start1>[0-9]*)-(?<end1>[0-9]*) or (?<start2>[0-9]*)-(?<end2>[0-9]*)", RegexOptions.Compiled);

        private static (IEnumerable<Rule> Rules, int[] MyTicket, IEnumerable<int[]> NearbyTickets) ProcessInput(string input)
        {
            var sections = input.Split(Environment.NewLine + Environment.NewLine);

            var rules = new List<Rule>();
            foreach (var rule in sections[0].Split(Environment.NewLine))
            {
                var groups = _ruleRegex.Matches(rule).Single().Groups;

                rules.Add(new Rule
                {
                    Name = groups["name"].Value,
                    Start1 = Int32.Parse(groups["start1"].Value),
                    End1 = Int32.Parse(groups["end1"].Value),
                    Start2 = Int32.Parse(groups["start2"].Value),
                    End2 = Int32.Parse(groups["end2"].Value),
                });
            }

            var myTicket = sections[1].Split(Environment.NewLine)[1].Split(',').Select(s => Int32.Parse(s)).ToArray();

            var nearbyTickets = sections[2].Split(Environment.NewLine).Skip(1).Select(s => s.Split(',').Select(s => Int32.Parse(s)).ToArray());

            return (rules, myTicket, nearbyTickets);
        }

        public class Rule
        {
            public string Name { get; init; }
            public int Start1 { get; init; }
            public int End1 { get; init; }
            public int Start2 { get; init; }
            public int End2 { get; init; }
            public bool IsValid(int x) => (Start1 <= x && x <= End1) || (Start2 <= x && x <= End2);
        }
    }
}