using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day4
    {
        public static void Run()
        {
            var passports = Utils.ReadFileAsString("Day4.txt")
                                 .Split(new[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(ParsePassport);

            Console.WriteLine($"Part 1: {Part1(passports)}");
            Console.WriteLine($"Part 2: {Part2(passports)}");
        }

        private static int Part1(IEnumerable<Passport> passports) => passports.Count(p => p.HasRequiredFields());

        private static int Part2(IEnumerable<Passport> passports) => passports.Count(p => p.IsValid());

        private static Passport ParsePassport(string input)
        {
            var fields = input.Replace(Environment.NewLine, " ")
                        .Split(" ")
                        .Select(s => s.Replace(Environment.NewLine, " ").Split(':'))
                        .ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            return new Passport(fields);
        }

        private class Passport
        {
            private Dictionary<string, string> _fields = new Dictionary<string, string>();

            public Passport(Dictionary<string, string> fields) => _fields = fields;

            public bool HasRequiredFields()
            {
                var requiredFields = new List<string> {
                    "ecl", "pid", "eyr", "hcl", "byr", "iyr", "hgt",
                };

                foreach (var requiredField in requiredFields)
                {
                    if (!_fields.ContainsKey(requiredField))
                        return false;
                }
                return true;
            }

            public bool IsValid() => HasRequiredFields()
                                  && IsValidBirthYear()
                                  && IsValidIssueYear()
                                  && IsValidExpirationYear()
                                  && IsValidHeight()
                                  && IsValidHairColor()
                                  && IsValidEyeColor()
                                  && IsValidPassportId();

            private bool IsValidBirthYear() => Int32.Parse(_fields["byr"]) is >= 1290 and <= 2002;

            private bool IsValidIssueYear() => Int32.Parse(_fields["iyr"]) is >= 2010 and <= 2020;

            private bool IsValidExpirationYear() => Int32.Parse(_fields["eyr"]) is >= 2020 and <= 2030;

            private bool IsValidHeight()
            {
                var heightRegex = new Regex(@"^(?<height>\d*)(?<unit>[a-z]*)", RegexOptions.Compiled);
                var matches = heightRegex.Matches(_fields["hgt"]);
                if (matches.Count == 0)
                    return false;

                var groups = matches.Single().Groups;
                var height = Int32.Parse(groups["height"].Value);

                return groups["unit"].Value switch
                {
                    "cm" => height is >= 150 and <= 193,
                    "in" => height is >= 59 and <= 76,
                    _ => false
                };
            }

            private bool IsValidHairColor()
            {
                var hairColorRegex = new Regex(@"^#([0-9a-fA-F]){6,6}$", RegexOptions.Compiled);
                return hairColorRegex.IsMatch(_fields["hcl"]);
            }

            private bool IsValidEyeColor()
            {
                var validColors = new List<string> {
                   "amb", "blu", "brn", "gry", "grn", "hzl", "oth"
                };

                return validColors.Contains(_fields["ecl"]);
            }

            private bool IsValidPassportId()
            {
                var passportIdRegex = new Regex(@"^([0-9]){9,9}$", RegexOptions.Compiled);
                return passportIdRegex.IsMatch(_fields["pid"]);
            }
        }
    }
}