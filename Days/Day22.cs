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
            var (playerOne, playerTwo) = ParsePlayerDecks(Utils.ReadFileAsString("Day22.txt"));

            var savedDeckOne = playerOne.Deck.ToList();
            var savedDeckTwo = playerTwo.Deck.ToList();

            Console.WriteLine($"Part 1: {Part1(playerOne, playerTwo)}");

            playerOne.ResetDeck(savedDeckOne);
            playerTwo.ResetDeck(savedDeckTwo);

            Console.WriteLine($"Part 2: {Part2(playerOne, playerTwo)}");
        }

        private static long Part1(Player playerOne, Player playerTwo)
        {
            while (!playerOne.HasEmptyDeck && !playerTwo.HasEmptyDeck)
            {
                var card1 = playerOne.DrawCard();
                var card2 = playerTwo.DrawCard();

                if (card1 > card2)
                    playerOne.AddToBottomOfDeck(card1, card2);
                else
                    playerTwo.AddToBottomOfDeck(card2, card1);
            }

            return playerOne.HasEmptyDeck ? playerTwo.CalculateScore() : playerOne.CalculateScore();
        }

        private static long Part2(Player playerOne, Player playerTwo) => PlayRecursiveGame(playerOne, playerTwo).CalculateScore();

        private static Player PlayRecursiveGame(Player playerOne, Player playerTwo)
        {
            var rounds = new List<Round>();

            while (!playerOne.HasEmptyDeck && !playerTwo.HasEmptyDeck)
            {
                if (rounds.Any(round => playerOne.IsSameDeck(round.PlayerOneDeck) || playerTwo.IsSameDeck(round.PlayerTwoDeck)))
                    return playerOne;

                var previousRound = new Round(playerOne.Deck.ToList(), playerTwo.Deck.ToList());
                rounds.Add(previousRound);

                var card1 = playerOne.DrawCard();
                var card2 = playerTwo.DrawCard();

                if (card1 <= playerOne.Deck.Count() && card2 <= playerTwo.Deck.Count())
                {
                    var winner = PlaySubGame(previousRound, card1, card2);
                    PerformRoundWin(winner == playerOne, card1, card2);
                }
                else
                {
                    PerformRoundWin(card1 > card2, card1, card2);
                }
            }

            Player PlaySubGame(Round previousRound, int card1, int card2)
            {
                playerOne.SetDeckToTopCards(card1);
                playerTwo.SetDeckToTopCards(card2);

                var winner = PlayRecursiveGame(playerOne, playerTwo);

                playerOne.ResetDeck(previousRound.PlayerOneDeck.Skip(1));
                playerTwo.ResetDeck(previousRound.PlayerTwoDeck.Skip(1));

                return winner;
            }

            void PerformRoundWin(bool isPlayerOne, int card1, int card2)
            {
                if (isPlayerOne)
                    playerOne.AddToBottomOfDeck(card1, card2);
                else
                    playerTwo.AddToBottomOfDeck(card2, card1);
            }

            return playerOne.HasEmptyDeck ? playerTwo : playerOne;
        }

        private static (Player player1, Player player2) ParsePlayerDecks(string input)
        {
            var players = input.Split(Environment.NewLine + Environment.NewLine);

            var playerOneDeck = players.First().Split(Environment.NewLine).Skip(1).Select(s => Int32.Parse(s.Trim()));
            var playerTwoDeck = players.Last().Split(Environment.NewLine).Skip(1).Select(s => Int32.Parse(s.Trim()));

            return (new Player(playerOneDeck), new Player(playerTwoDeck));
        }

        private record Round(List<int> PlayerOneDeck, List<int> PlayerTwoDeck);

        private class Player
        {
            public Player(IEnumerable<int> deck)
            {
                Deck = new Queue<int>(deck);
            }

            public Queue<int> Deck { get; private set; }

            public bool HasEmptyDeck => Deck.Count == 0;

            public bool IsSameDeck(IEnumerable<int> otherDeck) => Deck.SequenceEqual(otherDeck);

            public int DrawCard() => Deck.Dequeue();

            public void AddToBottomOfDeck(int cardOne, int cardTwo)
            {
                Deck.Enqueue(cardOne);
                Deck.Enqueue(cardTwo);
            }

            public void SetDeckToTopCards(int count) => ResetDeck(Deck.Take(count));

            public void ResetDeck(IEnumerable<int> deck) => Deck = new Queue<int>(deck);

            public long CalculateScore() => Deck.Reverse().Select((card, i) => card * (i + 1)).Sum();

        }
    }
}