using System;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode2020.Days
{
    public static class Day14
    {
        public static void Run()
        {
            var input = Utils.ReadFromFile("Day14.txt").Select(ParseInput).ToList();

            Console.WriteLine($"Part 1: {Part1(input)}");
            Console.WriteLine($"Part 2: {Part2(input)}");
        }

        private static long Part1(List<ICommand> commands)
        {
            var mem = new Dictionary<string, long>();
            var mask = string.Empty;

            foreach (var command in commands)
            {
                if (command is MaskCommand)
                    mask = command.Value;
                else if (command is MemCommand memCommand)
                {
                    var binaryValue = LongToBinary(Convert.ToInt64(memCommand.Value), bitLength: 36);

                    for (var i = 0; i < mask.Length; i++)
                    {
                        var c = mask[i];

                        if (c is 'X')
                            continue;

                        binaryValue = binaryValue.Remove(i, 1).Insert(i, c.ToString());
                    }

                    mem[memCommand.Location] = BinaryToLong(binaryValue);
                }
            }

            return mem.Values.Sum();
        }

        private static long Part2(List<ICommand> commands)
        {
            var mem = new Dictionary<string, long>();
            var mask = string.Empty;

            foreach (var command in commands)
            {
                if (command is MaskCommand)
                    mask = command.Value;
                else if (command is MemCommand memCommand)
                {
                    var baseKey = LongToBinary(Convert.ToInt64(memCommand.Location), 36);

                    List<string> keys = mask[0] switch
                    {
                        '0' => new() { baseKey[0].ToString() },
                        '1' => new() { "1" },
                        'X' => new() { "0", "1" }
                    };

                    for (var i = 1; i < mask.Length; i++)
                    {
                        var c = mask[i];

                        if (c is '0')
                            keys = keys.Select(k => k += baseKey[i]).ToList();
                        else if (c is '1')
                            keys = keys.Select(k => k += c).ToList();
                        else if (c is 'X')
                        {
                            var keysCount = keys.Count();
                            for (var j = 0; j < keysCount; j++)
                            {
                                var key = keys[j];
                                keys.Add(key + '1');
                                keys[j] += '0';
                            }
                        }
                    }

                    var value = Convert.ToInt64(memCommand.Value);
                    keys.ForEach(key => mem[key] = value);
                }
            }

            return mem.Values.Sum();
        }

        private interface ICommand
        {
            string Value { get; init; }
        }

        private record MaskCommand(string Value) : ICommand;
        private record MemCommand(string Location, string Value) : ICommand;

        private static ICommand ParseInput(string s)
        {
            var split = s.Trim().Split(" = ");

            var command = split[0];
            var value = split[1];

            if (command == "mask")
                return new MaskCommand(value);
            else if (command.StartsWith("mem"))
            {
                var location = string.Concat(command.Skip(4).TakeWhile(char.IsDigit));
                return new MemCommand(location, value);
            }

            throw new ArgumentException("Command not supported");
        }

        private static long BinaryToLong(string s) => Convert.ToInt64(s, fromBase: 2);

        private static string LongToBinary(long l, int bitLength) => Convert.ToString(l, toBase: 2).PadLeft(bitLength, '0');

    }
}