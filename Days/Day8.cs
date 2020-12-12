using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day8
    {
        public static void Run()
        {
            var instructions = Utils.ReadFromFile("Day8.txt").Select(ParseInstruction).ToArray();

            Console.WriteLine($"Part 1: {Part1(instructions)}");
            Console.WriteLine($"Part 2: {Part2(instructions)}");
        }

        private static int Part1(Instruction[] instructions)
        {
            var acc = 0;

            PerformInstruction(0);

            return acc;

            void PerformInstruction(int index)
            {
                var instruction = instructions[index];
                if (instruction.Visited)
                    return;

                instruction.Visited = true;

                switch (instruction.Type)
                {
                    case "acc":
                        acc += instruction.Number;
                        PerformInstruction(index + 1);
                        break;
                    case "nop":
                        PerformInstruction(index + 1);
                        break;
                    case "jmp":
                        PerformInstruction(index + instruction.Number);
                        break;
                }
            }
        }

        private static int Part2(Instruction[] instructions)
        {
            int acc;

            for (var i = 0; i < instructions.Count(); i++)
            {
                if (instructions[i].Type == "acc")
                    continue;

                instructions[i].Type = instructions[i].Type == "jmp" ? "nop" : "jmp";

                acc = 0;

                foreach (var instruction in instructions)
                    instruction.Visited = false;

                var result = PerformInstruction(0);

                if (result)
                    return acc;

                instructions[i].Type = instructions[i].Type == "jmp" ? "nop" : "jmp";
            }

            return -1;

            bool PerformInstruction(int index)
            {
                if (index == instructions.Count())
                    return true;

                var instruction = instructions[index];
                if (instruction.Visited)
                    return false;

                instruction.Visited = true;

                switch (instruction.Type)
                {
                    case "acc":
                        acc += instruction.Number;
                        return PerformInstruction(index + 1);
                    case "nop":
                        return PerformInstruction(index + 1);
                    case "jmp":
                        return PerformInstruction(index + instruction.Number);
                    default:
                        throw new ArgumentException();
                }
            }
        }

        private static Regex _instructionRegex = new Regex(@"^(?<type>[a-z]{3}) (?<number>.[0-9]*)$", RegexOptions.Compiled);
        private static Instruction ParseInstruction(string instructionString)
        {
            var groups = _instructionRegex.Matches(instructionString).Single().Groups;

            return new Instruction()
            {
                Type = groups["type"].Value,
                Number = int.Parse(groups["number"].Value, System.Globalization.NumberStyles.AllowLeadingSign),
                Visited = false,
            };
        }

        private class Instruction
        {
            public string Type { get; set; }
            public int Number { get; init; }
            public bool Visited { get; set; }
        }
    }
}