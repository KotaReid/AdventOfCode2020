using System;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day11
    {
        public static void Run()
        {
            var seats = Utils.ReadFromFile("Day11.txt").Select(ParseSeatLayout);

            Console.WriteLine($"Part 1: {Part1(seats.ToArray())}");
            Console.WriteLine($"Part 2: {Part2(seats.ToArray())}");
        }

        private static int Part1(Seat[][] seats)
        {
            while (PerformRoundPart1(seats)) ;

            return seats.Select(row => row.Count(s => s is not null && s.IsOccupied)).Sum();
        }

        private static bool PerformRoundPart1(Seat[][] seats)
        {
            var rowCount = seats.Length;
            var colCount = seats[0].Length;
            var wasUpdated = false;

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var seat = seats[row][col];

                    if (seat is null)
                        continue;

                    var adjacentSeats = new[] {
                        TryGetAdjacentSeat(row-1,col-1),
                        TryGetAdjacentSeat(row-1,col),
                        TryGetAdjacentSeat(row-1,col+1),
                        TryGetAdjacentSeat(row,col-1),
                        TryGetAdjacentSeat(row,col+1),
                        TryGetAdjacentSeat(row+1,col-1),
                        TryGetAdjacentSeat(row+1,col),
                        TryGetAdjacentSeat(row+1,col+1),
                    };

                    var occupiedSeats = adjacentSeats.Where(s => s is not null && s.IsOccupied).Count();

                    if ((seat.IsOccupied && occupiedSeats >= 4) || (!seat.IsOccupied && occupiedSeats == 0))
                        seat.ShouldUpdate = true;
                }
            }

            foreach (var row in seats)
            {
                foreach (var seat in row)
                {
                    if (seat is null)
                        continue;

                    if (seat.ShouldUpdate)
                    {
                        wasUpdated = true;
                        seat.IsOccupied = !seat.IsOccupied;
                        seat.ShouldUpdate = false;
                    }
                }
            }

            return wasUpdated;

            Seat TryGetAdjacentSeat(int row, int col)
            {
                if (row < 0 || row >= rowCount || col < 0 || col >= colCount)
                    return null;

                return seats[row][col];
            }
        }

        private static int Part2(Seat[][] seats)
        {
            while (PerformRoundPart2(seats)) ;

            return seats.Select(row => row.Count(s => s is not null && s.IsOccupied)).Sum();
        }

        private static bool PerformRoundPart2(this Seat[][] seats)
        {
            var rowCount = seats.Length;
            var colCount = seats[0].Length;
            var wasUpdated = false;

            for (var row = 0; row < rowCount; row++)
            {
                for (var col = 0; col < colCount; col++)
                {
                    var seat = seats[row][col];

                    if (seat is null)
                        continue;

                    var adjacentSeats = new[] {
                        TryGetAdjacentSeat(row, col, Direction.N),
                        TryGetAdjacentSeat(row, col, Direction.NE),
                        TryGetAdjacentSeat(row, col, Direction.E),
                        TryGetAdjacentSeat(row, col, Direction.SE),
                        TryGetAdjacentSeat(row, col, Direction.S),
                        TryGetAdjacentSeat(row, col, Direction.SW),
                        TryGetAdjacentSeat(row, col, Direction.W),
                        TryGetAdjacentSeat(row, col, Direction.NW),
                    };

                    var occupiedSeats = adjacentSeats.Where(s => s is not null && s.IsOccupied).Count();

                    if ((seat.IsOccupied && occupiedSeats >= 5) || (!seat.IsOccupied && occupiedSeats == 0))
                        seat.ShouldUpdate = true;
                }
            }

            foreach (var row in seats)
            {
                foreach (var seat in row)
                {
                    if (seat is null)
                        continue;

                    if (seat.ShouldUpdate)
                    {
                        wasUpdated = true;
                        seat.IsOccupied = !seat.IsOccupied;
                        seat.ShouldUpdate = false;
                    }
                }
            }

            return wasUpdated;

            Seat TryGetAdjacentSeat(int row, int col, Direction direction)
            {
                while (true)
                {
                    var (x, y) = direction switch
                    {
                        Direction.N => (0, 1),
                        Direction.NE => (1, 1),
                        Direction.E => (1, 0),
                        Direction.SE => (1, -1),
                        Direction.S => (0, -1),
                        Direction.SW => (-1, -1),
                        Direction.W => (-1, 0),
                        Direction.NW => (-1, 1),
                        _ => throw new ArgumentException("Unknown direction")
                    };

                    col += x;
                    row += y;

                    if (row < 0 || row >= rowCount || col < 0 || col >= colCount)
                        return null;

                    var seat = seats[row][col];

                    if (seat is null)
                        continue;

                    return seat;
                }
            }
        }

        private static Seat[] ParseSeatLayout(string s)
        {
            s = s.Trim();
            var seats = new Seat[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                seats[i] = s[i] switch
                {
                    '.' => null,
                    'L' => new Seat { IsOccupied = false },
                    '#' => new Seat { IsOccupied = true },
                    _ => throw new ArgumentException("Layout not defined")
                };
            }

            return seats;
        }

        private class Seat
        {
            public bool IsOccupied { get; set; }
            public bool ShouldUpdate { get; set; }
        }

        private enum Direction
        {
            N,
            NE,
            E,
            SE,
            S,
            SW,
            W,
            NW,
        }
    }
}