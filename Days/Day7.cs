using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day7
    {
        public static void Run()
        {
            var bagRules = Utils.ReadFromFile("Day7.txt").Select(ParseBag)
                .ToDictionary(b => b.ParentName, b => b.ChildBags);

            Console.WriteLine($"Part 1: {Part1(bagRules)}");
            Console.WriteLine($"Part 2: {Part2(bagRules)}");
        }

        private static int Part1(Dictionary<string, Dictionary<string, int>> bagRules)
        {
            var seenBags = new HashSet<string>();
            FindBags("shiny gold");

            return seenBags.Count;

            void FindBags(string name)
            {
                var foundParents = bagRules.Where(x => x.Value.ContainsKey(name)).Select(x => x.Key).ToList();

                if (!foundParents.Any())
                    return;

                foreach (var parent in foundParents)
                {
                    if (!seenBags.Contains(parent))
                    {
                        seenBags.Add(parent);
                        FindBags(parent);
                    }
                }
            }
        }

        private static int Part2(Dictionary<string, Dictionary<string, int>> bagRules)
        {
            var answers = new Dictionary<string, int>();

            return GetBagCount(bagRules["shiny gold"]) - 1;

            int GetBagCount(Dictionary<string, int> childBags)
            {
                var answer = 1;
                foreach (var childBag in childBags)
                {
                    if (!answers.TryGetValue(childBag.Key, out var childAnswer))
                    {
                        childAnswer = GetBagCount(bagRules[childBag.Key]);
                        answers.Add(childBag.Key, childAnswer);
                    }

                    answer += childBag.Value * childAnswer;
                }

                return answer;
            }
        }

        private static Regex _bagRegex = new Regex(@"^(?<parentName>.*)bags contain (?<childBags>.*).$", RegexOptions.Compiled);
        private static Regex _childRegex = new Regex(@"^(?<count>[0-9]+) (?<name>.*)bags?", RegexOptions.Compiled);
        private static(string ParentName, Dictionary<string, int> ChildBags) ParseBag(string bagString)
        {
            var groups = _bagRegex.Matches(bagString).Single().Groups;

            var parentName = groups["parentName"].Value.Trim();
            var childBags = groups["childBags"].Value
                .Split(',')
                .Select(s =>
                {
                    var matches = _childRegex.Matches(s.Trim());
                    return matches.Count == 0 ? null : matches.Single().Groups;
                })
                .Where(g => g is not null)
                .ToDictionary(g => g["name"].Value.Trim(), g => Int32.Parse(g["count"].Value.Trim()));

            return (parentName, childBags);
        }
    }
}