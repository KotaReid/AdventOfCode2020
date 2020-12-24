using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day24
    {
        public static void Run()
        {
            var paths = Utils.ReadFromFile("Day24.txt").Select(s => s.Trim()).ToList();

            var floor = new Floor();
            paths.ForEach(path => floor.TraversePath(path));

            Console.WriteLine($"Part 1: {Part1(floor)}");
            Console.WriteLine($"Part 2: {Part2(floor)}");
        }

        private static int Part1(Floor floor) => floor.BlackTilesCount();

        private static int Part2(Floor floor)
        {
            floor.GameOfLife();
            return floor.BlackTilesCount();
        }

        private static Dictionary<string, TileDirection> _parseDirectionMap = new()
        {
            { "e", TileDirection.East },
            { "w", TileDirection.West },
            { "ne", TileDirection.NorthEast },
            { "nw", TileDirection.NorthWest },
            { "se", TileDirection.SouthEast },
            { "sw", TileDirection.SouthWest },
        };

        private static Dictionary<TileDirection, Point> _neighborMap = new()
        {
            { TileDirection.West, new(1, -1, 0) },
            { TileDirection.East, new(-1, 1, 0) },
            { TileDirection.SouthWest, new(-1, 0, 1) },
            { TileDirection.NorthEast, new(1, 0, -1) },
            { TileDirection.SouthEast, new(0, -1, 1) },
            { TileDirection.NorthWest, new(0, 1, -1) },
        };

        private enum TileDirection
        {
            East,
            West,
            NorthEast,
            NorthWest,
            SouthEast,
            SouthWest
        }

        private class Floor
        {
            private Point _centerPoint = new Point(0, 0, 0);
            private Dictionary<Point, bool> _tiles = new();

            public Floor() => _tiles.Add(_centerPoint, false);

            public void TraversePath(string path)
            {
                var x = 0;
                var y = 0;
                var z = 0;

                for (var i = 0; i < path.Length; i++)
                {
                    if (!_parseDirectionMap.TryGetValue(path.Substring(i, 1), out var direction))
                        direction = _parseDirectionMap[path.Substring(i, 2)];

                    var deltaPoint = _neighborMap[direction];

                    x += deltaPoint.X;
                    y += deltaPoint.Y;
                    z += deltaPoint.Z;
                }

                var targetPoint = new Point(x, y, z);

                if (_tiles.ContainsKey(targetPoint))
                    _tiles[targetPoint] = !_tiles[targetPoint];
                else
                    _tiles[targetPoint] = true;
            }

            public void GameOfLife()
            {
                RemoveWhiteTiles();
                var blackPoints = _tiles.Keys.ToHashSet();

                for (var i = 1; i <= 100; i++)
                {
                    var newBlackPoints = new List<Point>();

                    foreach (var point in blackPoints)
                    {
                        var neighborPoints = _neighborMap.Values.Select(p => new Point(point.X + p.X, point.Y + p.Y, point.Z + p.Z));

                        if (neighborPoints.Count(np => blackPoints.Contains(np)) is 1 or 2)
                            newBlackPoints.Add(point);

                        foreach (var whitePoint in neighborPoints.Where(np => !blackPoints.Contains(np)))
                        {
                            var whiteNeighborPoints = _neighborMap.Values.Select(p => new Point(whitePoint.X + p.X, whitePoint.Y + p.Y, whitePoint.Z + p.Z));
                            if (whiteNeighborPoints.Count(np => blackPoints.Contains(np)) is 2)
                                newBlackPoints.Add(whitePoint);
                        }
                    }

                    blackPoints = newBlackPoints.ToHashSet();
                }

                _tiles = blackPoints.ToDictionary(point => point, _ => true);
            }

            public int BlackTilesCount() => _tiles.Values.Count(isBlack => isBlack);

            private void RemoveWhiteTiles()
            {
                foreach (var tileToRemove in _tiles.Where(kvp => !kvp.Value).Select(kvp => kvp.Key))
                    _tiles.Remove(tileToRemove);
            }
        }

        private record Point(int X, int Y, int Z);
    }
}