using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day22
    {
        public static void Run()
        {
            var (p1, p2) = ParsePlayerDecks(Utils.ReadFileAsString("Day22.txt"));

            Console.WriteLine($"Part 1: {Part1(new Queue<int>(p1), new Queue<int>(p2))}");
            Console.WriteLine($"Part 2: {Part2(p1, p2)}");
        }

        private static long Part1(Queue<int> p1, Queue<int> p2)
        {
            while (p1.Count > 0 && p2.Count > 0)
            {
                var card1 = p1.Dequeue();
                var card2 = p2.Dequeue();

                if (card1 > card2)
                {
                    p1.Enqueue(card1);
                    p1.Enqueue(card2);
                }
                else
                {
                    p2.Enqueue(card2);
                    p2.Enqueue(card1);
                }
            }

            return p1.Count > 0 ? CalculateScore(p1) : CalculateScore(p2);
        }

        private enum Winner
        {
            Player1,
            Player2
        }

        private static long Part2(Queue<int> p1, Queue<int> p2)
        {
            var (winner, player1, player2) = PlayRecursiveGame(new Queue<int>(p1), new Queue<int>(p2));
            return winner is Winner.Player1 ? CalculateScore(player1) : CalculateScore(player2);
        }

        private static (Winner Winner, Queue<int> Player1, Queue<int> Player2) PlayRecursiveGame(Queue<int> playerOne, Queue<int> playerTwo)
        {
            var rounds = new List<(List<int> Player1, List<int> Player2)>();

            while (playerOne.Count > 0 && playerTwo.Count > 0)
            {
                foreach (var round in rounds)
                {
                    if ((playerOne.Count == round.Player1.Count() && Enumerable.SequenceEqual(playerOne, round.Player1)) ||
                        (playerTwo.Count == round.Player2.Count() && Enumerable.SequenceEqual(playerTwo, round.Player2)))
                    {
                        return (Winner.Player1, playerOne, playerTwo);
                    }
                }

                rounds.Add((playerOne.ToList(), playerTwo.ToList()));

                var card1 = playerOne.Dequeue();
                var card2 = playerTwo.Dequeue();

                if (card1 <= playerOne.Count() && card2 <= playerTwo.Count())
                {
                    var winner = PlayRecursiveGame(new Queue<int>(playerOne.Take(card1)), new Queue<int>(playerTwo.Take(card2))).Winner;

                    if (winner is Winner.Player1)
                    {
                        playerOne.Enqueue(card1);
                        playerOne.Enqueue(card2);
                    }
                    else
                    {
                        playerTwo.Enqueue(card2);
                        playerTwo.Enqueue(card1);
                    }
                }
                else
                {
                    if (card1 > card2)
                    {
                        playerOne.Enqueue(card1);
                        playerOne.Enqueue(card2);
                    }
                    else
                    {
                        playerTwo.Enqueue(card2);
                        playerTwo.Enqueue(card1);
                    }
                }
            }

            return (playerOne.Count > 0 ? Winner.Player1 : Winner.Player2, playerOne, playerTwo);
        }

        private static long CalculateScore(Queue<int> winningDeck)
        {
            var score = 0;
            foreach (var card in winningDeck.Reverse().Select((num, i) => (Value: num, Multiplier: i + 1)))
                score += (card.Value * card.Multiplier);

            return score;
        }

        private static (Queue<int> PlayerOneDeck, Queue<int> PlayerTwoDeck) ParsePlayerDecks(string input)
        {
            var players = input.Split(Environment.NewLine + Environment.NewLine);

            var playerOneDeck = players.First().Split(Environment.NewLine).Skip(1).Select(s => Int32.Parse(s.Trim()));
            var playerTwoDeck = players.Last().Split(Environment.NewLine).Skip(1).Select(s => Int32.Parse(s.Trim()));

            return (new Queue<int>(playerOneDeck), new Queue<int>(playerTwoDeck));
        }
    }
}