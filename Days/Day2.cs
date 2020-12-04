using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day2
    {
        private record Password(int FirstPolicy, int SecondPolicy, char PolicyChar, string Value);

        public static void Run()
        {
            var passwords = Utils.ReadFromFile("Day2.txt").Select(ToPassword).ToList();

            Console.WriteLine($"Part 1: {Part1(passwords)}");
            Console.WriteLine($"Part 2: {Part2(passwords)}");
        }

        private static int Part1(List<Password> passwords)
        {
            var numberValid = 0;
            foreach (var password in passwords)
            {
                var number = password.Value.Count(c => c == password.PolicyChar);

                if (number >= password.FirstPolicy && number <= password.SecondPolicy)
                    numberValid++;
            }

            return numberValid;
        }

        private static int Part2(List<Password> passwords)
        {
            var numberValid = 0;
            foreach (var password in passwords)
            {
                var char1 = password.Value[password.FirstPolicy - 1];
                var char2 = password.Value[password.SecondPolicy - 1];

                if (char1 != char2 && (char1 == password.PolicyChar || char2 == password.PolicyChar))
                    numberValid++;
            }

            return numberValid;
        }

        private static Regex regex = new Regex(@"^(?<firstPolicy>\d*)-(?<secondPolicy>\d*)(\s*)(?<policy>.):(\s*)(?<password>.*)$", RegexOptions.Compiled);
        private static Password ToPassword(this string s)
        {
            var matches = regex.Matches(s);
            var groups = matches.Single().Groups;
            var firstPolicy = Int32.Parse(groups["firstPolicy"].Value);
            var secondPolicy = Int32.Parse(groups["secondPolicy"].Value);
            var policy = groups["policy"].Value.First();
            var password = groups["password"].Value;

            return new Password(firstPolicy, secondPolicy, policy, password);
        }
    }
}