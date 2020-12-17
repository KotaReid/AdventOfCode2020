using System.Collections.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day17
    {
        public static void Run()
        {
            var activeCubes3D = ParseCubes3D(Utils.ReadFileAsString("Day17.txt")).ToImmutableList();
            Console.WriteLine($"Part 1: {Part1(activeCubes3D)}");

            var activeCubes4D = activeCubes3D.Select(cube => new Point(cube.Positions.Append(0).ToList())).ToImmutableList();
            Console.WriteLine($"Part 2: {Part2(activeCubes4D)}");
        }

        private static int Part1(ImmutableList<Point> activeCubes)
        {
            var directionVectors3D = (from x in Enumerable.Range(-1, 3)
                                      from y in Enumerable.Range(-1, 3)
                                      from z in Enumerable.Range(-1, 3)
                                      where !(x == 0 && y == 0 && z == 0)
                                      select new Point(new List<int> { x, y, z }))
                                    .ToImmutableList();

            for (var i = 1; i <= 6; i++)
                activeCubes = GetNewActiveCubes(activeCubes, directionVectors3D);

            return activeCubes.Count();
        }

        private static int Part2(ImmutableList<Point> activeCubes)
        {
            var directionVectors4D = (from x in Enumerable.Range(-1, 3)
                                      from y in Enumerable.Range(-1, 3)
                                      from z in Enumerable.Range(-1, 3)
                                      from w in Enumerable.Range(-1, 3)
                                      where !(x == 0 && y == 0 && z == 0 && w == 0)
                                      select new Point(new List<int> { x, y, z, w }))
                                    .ToImmutableList();

            for (var i = 1; i <= 6; i++)
                activeCubes = GetNewActiveCubes(activeCubes, directionVectors4D);

            return activeCubes.Count();
        }

        private static ImmutableList<Point> GetNewActiveCubes(ImmutableList<Point> activeCubes, ImmutableList<Point> directionVectors)
        {
            var newActiveCubes = new List<Point>();
            var cubesToCheck = new Dictionary<string, Point>();

            foreach (var activeCube in activeCubes)
            {
                cubesToCheck.TryAdd(activeCube.ToString(), activeCube);

                foreach (var neighbor in directionVectors.Select(vector => activeCube.Move(vector)))
                    cubesToCheck.TryAdd(neighbor.ToString(), neighbor);
            }

            foreach (var cubeToCheck in cubesToCheck.Values)
            {
                if (NextCubeActive(cubeToCheck))
                    newActiveCubes.Add(cubeToCheck);
            }

            return newActiveCubes.ToImmutableList();

            bool NextCubeActive(Point cubeToCheck)
            {
                var count = 0;
                foreach (var activeCube in activeCubes)
                {
                    if (IsNeighbor(cubeToCheck, activeCube))
                        count++;

                    if (count > 3)
                        return false;
                }

                var cube = activeCubes.SingleOrDefault(c => c.IsSame(cubeToCheck));
                var isActive = cube is null ? false : true;
                return (isActive && (count is 2 or 3)) || count is 3;
            }

            bool IsNeighbor(Point source, Point other) => !other.IsSame(source)
                                                     && !other.Positions.Zip(source.Positions, (o, s) => o.InRange(s - 1, s + 1)).Any(b => !b);
        }
        private static bool InRange(this int x, int min, int max) => min <= x && x <= max;

        private static IEnumerable<Point> ParseCubes3D(string input)
        {
            var lines = input.Split(Environment.NewLine);

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                        yield return new Point(new List<int> { x, y, 0 });
                }
            }
        }

        private class Point
        {
            public Point(List<int> positions) => Positions = positions;

            public List<int> Positions { get; }

            public bool IsSame(Point otherPoint) => ToString() == otherPoint.ToString();
            public Point Move(Point vector) => new Point(Positions.Zip(vector.Positions, (p, v) => p += v).ToList());

            public override string ToString() => string.Join(',', Positions.Select(p => p));
        }
    }
}