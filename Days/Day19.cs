using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day19
    {
        public static void Run()
        {
            var (rules, messages) = ParseRules(Utils.ReadFileAsString("Day19.txt"));
            Console.WriteLine($"Part 1: {Part1(rules, messages)}");
            Console.WriteLine($"Part 2: {Part2(rules, messages)}");
        }

        private static long Part1(Dictionary<int, Rule> rules, List<string> messages)
        {
            var regex = new Regex($@"\b{BuildRegexSection(0, rules)}\b", RegexOptions.Compiled);
            return messages.Count(regex.IsMatch);
        }

        private static long Part2(Dictionary<int, Rule> rules, List<string> messages)
        {

            var s42 = BuildRegexSection(42, rules);
            var s31 = BuildRegexSection(31, rules);

            //8 = 42 | 42 8
            var s8 = $"({s42})+";

            //TIL: .Net Regex has balancing groups to track stacking
            //First group count puts in stack
            //Second group count pop off stack (-COUNT)
            //Third group is negative lookahead that is only valid if nothing is on that stack
            //11 = 42 31 | 42 11 31
            var s11 = $@"((?<COUNT>({s42}))+(?<-COUNT>({s31}))+(?(COUNT)(?!)))";

            //0 = 8 11
            var s0 = $@"\b(({s8})({s11}))\b";
            var regex = new Regex(s0, RegexOptions.Compiled);

            var validCount = 0;
            foreach (var message in messages)
            {
                var matches = regex.Matches(message);

                if (matches.Any())
                {
                    var match = matches.Single();
                    var part1 = match.Groups["part1"].Captures.Count();
                    var part2 = match.Groups["part2"].Captures.Count();

                    if (part1 == part2)
                        validCount++;
                }
            }

            return validCount;
        }

        private static (Dictionary<int, Rule> Rules, List<string> messages) ParseRules(string input)
        {
            var split = input.Split(Environment.NewLine + Environment.NewLine);

            var rules = new Dictionary<int, Rule>();
            foreach (var line in split[0].Split(Environment.NewLine))
            {
                var ruleParts = line.Split(':');
                var index = Int32.Parse(ruleParts[0].Trim());
                var value = ruleParts[1].Replace('"', ' ').Trim();
                rules.Add(index, new Rule(value));
            }

            return (rules, split[1].Split(Environment.NewLine).ToList());
        }

        private static string BuildRegexSection(int ruleIndex, Dictionary<int, Rule> rules)
        {
            var rule = rules[ruleIndex];

            if (rule.IsSolved)
                return rule.Condition;

            var s = string.Empty;

            if (rule.Condition.Contains('|'))
            {
                var conditions = rule.Condition.Split(" | ");

                s += "((";
                ProcessSections(conditions.First());
                s += ")|(";
                ProcessSections(conditions.Last());
                s += "))";
            }
            else if (rule.Condition.All(c => char.IsLetter(c)))
                s += rule.Condition;
            else
                ProcessSections(rule.Condition);

            rules[ruleIndex].Condition = s;
            rules[ruleIndex].IsSolved = true;

            return s;

            void ProcessSections(string condition)
            {
                foreach (var item in condition.Split(" "))
                    s += $"{BuildRegexSection(Int32.Parse(item), rules)}";
            }
        }

        private class Rule
        {
            public Rule(string input)
            {
                Condition = input;
            }

            public string Condition { get; set; }
            public bool IsSolved { get; set; } = false;

            public override string ToString() => Condition;
        }
    }
}