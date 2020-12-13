using System.Globalization;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode2020.Days
{
    public static class Day13
    {
        public static void Run()
        {
            var input = Utils.ReadFromFile("Day13.txt").Select(s => s.Trim()).ToList();
            var targetTime = Int32.Parse(input[0]);
            var busIds = input[1].Split(',').Select(s => s == "x" ? 0 : Int32.Parse(s)).ToList();

            Console.WriteLine($"Part 1: {Part1(targetTime, busIds)}");
            Console.WriteLine($"Part 2: {Part2(busIds)}");
        }

        private static long Part1(int targetTime, List<int> busIds)
        {
            var minTime = Int64.MaxValue;
            var minBusId = 0;
            foreach (var busId in busIds)
            {
                if (busId == 0)
                    continue;

                var time = (long)Math.Ceiling((double)targetTime / busId) * busId;

                if (time < minTime)
                {
                    minTime = time;
                    minBusId = busId;
                }
            }

            return minBusId * (minTime - targetTime);
        }

        private static long Part2(List<int> busIds)
        {
            var time = (long)busIds.First();
            var timeToIncrementBy = (long)busIds.First();

            var buses = busIds.Select((busId, index) => (BusId: busId, Index: index))
                              .Where(b => b.BusId != 0);

            var busRanges = Enumerable.Range(1, buses.Count()).Select(i => buses.Take(i));

            foreach (var busRange in busRanges)
            {
                while (!busRange.All(b => (time + b.Index) % b.BusId == 0))
                    time += timeToIncrementBy;

                timeToIncrementBy = busRange.Select(b => (long)b.BusId).Aggregate(LCM);
            }

            return time;
        }

        private static long GCD(long x, long y) => x == 0 ? y : GCD(y % x, x);

        private static long LCM(long x, long y) => (x / GCD(x, y)) * y;
    }
}