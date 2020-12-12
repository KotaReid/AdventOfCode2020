using System.Net.Http;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace AdventOfCode2020.Days
{
    public static class Day12
    {
        public static void Run()
        {
            var instructions = Utils.ReadFromFile("Day12.txt").Select(ParseInstruction);

            Console.WriteLine($"Part 1: { Part1(instructions)}");
            Console.WriteLine($"Part 2: {Part2(instructions)}");
        }

        private static int Part1(IEnumerable<Instruction> instructions)
        {
            var ship = new Point(0, 0);
            var shipDirection = ShipDirection.E;

            foreach (var instruction in instructions)
            {
                if (instruction.Command is 'L' or 'R')
                {
                    shipDirection = shipDirection.RotateShip(instruction);
                }
                else
                {
                    var direction = instruction.Command is 'F' ? shipDirection : instruction.Command.ToDirection();

                    var (xDelta, yDelta) = MoveShip(instruction, direction);

                    ship.X += xDelta;
                    ship.Y += yDelta;
                }
            }

            return Math.Abs(ship.X) + Math.Abs(ship.Y);
        }

        private static ShipDirection ToDirection(this char c) => c switch
        {
            'N' => ShipDirection.N,
            'S' => ShipDirection.S,
            'E' => ShipDirection.E,
            'W' => ShipDirection.W,
            _ => throw new ArgumentException("Direction not supported")
        };

        private static ShipDirection RotateShip(this ShipDirection shipDirection, Instruction instruction)
        {
            List<ShipDirection> directionOrder = instruction.Command is 'L'
                ? new List<ShipDirection>()
                    {
                        ShipDirection.N,
                        ShipDirection.W,
                        ShipDirection.S,
                        ShipDirection.E,
                    }
                : new List<ShipDirection>()
                    {
                        ShipDirection.N,
                        ShipDirection.E,
                        ShipDirection.S,
                        ShipDirection.W,
                    };

            var index = directionOrder.IndexOf(shipDirection);

            index += instruction.Units switch
            {
                0 => 0,
                90 => 1,
                180 => 2,
                270 => 3,
                _ => throw new ArgumentException("Angle not defined")
            };

            return directionOrder[index % directionOrder.Count];
        }

        private static (int, int) MoveShip(Instruction instruction, ShipDirection shipDirection) => shipDirection switch
        {
            ShipDirection.N => (0, instruction.Units),
            ShipDirection.S => (0, -instruction.Units),
            ShipDirection.E => (instruction.Units, 0),
            ShipDirection.W => (-instruction.Units, 0),
            _ => throw new ArgumentException("Direction not supported")
        };

        private static int Part2(IEnumerable<Instruction> instructions)
        {
            var ship = new Point(0, 0);
            var wayPoint = new Point(10, 1);

            foreach (var instruction in instructions)
            {
                switch (instruction.Command)
                {
                    case 'F':
                        ship.X += wayPoint.X * instruction.Units;
                        ship.Y += wayPoint.Y * instruction.Units;
                        break;
                    case 'L' or 'R':
                        wayPoint = wayPoint.RotateWayPoint(instruction);
                        break;
                    default:
                        wayPoint = wayPoint.MoveWayPoint(instruction);
                        break;
                }
            }

            return Math.Abs(ship.X) + Math.Abs(ship.Y);
        }

        private static Point RotateWayPoint(this Point wayPoint, Instruction instruction) => (instruction.Units, instruction.Command) switch
        {
            (270, 'R') or (90, 'L') => new Point(-wayPoint.Y, wayPoint.X),
            (180, _) => new Point(-wayPoint.X, -wayPoint.Y),
            (90, 'R') or (270, 'L') => new Point(wayPoint.Y, -wayPoint.X),
            _ => throw new ArgumentException("Angle out of scope")
        };

        private static Point MoveWayPoint(this Point wayPoint, Instruction instruction) => instruction.Command switch
        {
            'N' => new Point(wayPoint.X, wayPoint.Y + instruction.Units),
            'S' => new Point(wayPoint.X, wayPoint.Y - instruction.Units),
            'E' => new Point(wayPoint.X + instruction.Units, wayPoint.Y),
            'W' => new Point(wayPoint.X - instruction.Units, wayPoint.Y),
            _ => throw new ArgumentException("Instruction out of scope")
        };

        private static Instruction ParseInstruction(string input)
        {
            input.Trim();
            return new Instruction(input[0], Int32.Parse(input.Substring(1)));
        }

        private record Instruction(char Command, int Units);

        private enum ShipDirection
        {
            N,
            E,
            S,
            W,
        }
    }
}