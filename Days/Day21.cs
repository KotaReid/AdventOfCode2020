using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day21
    {
        public static void Run()
        {
            var foodItems = Utils.ReadFromFile("Day21.txt").Select(ParseFood);

            var (knownAllergens, safeIngredients) = FindAllergens(foodItems);
            Console.WriteLine($"Part 1: {Part1(foodItems, safeIngredients)}");
            Console.WriteLine($"Part 2: {Part2(foodItems, knownAllergens)}");
        }

        private static int Part1(IEnumerable<Food> foodItems, HashSet<string> safeIngredients)
            => foodItems.Sum(food => food.Ingredients.Count(i => safeIngredients.Contains(i)));

        private static string Part2(IEnumerable<Food> foodItems, Dictionary<string, string> knownAllergens)
            => string.Join(',', knownAllergens.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

        private static (Dictionary<string, string> KnownAllergens, HashSet<string> SafeIngredients) FindAllergens(IEnumerable<Food> foodItems)
        {
            var knownAllergens = new Dictionary<string, string>();
            var safeIngredients = foodItems.SelectMany(f => f.Ingredients).Distinct().ToHashSet();
            var allergensCount = foodItems.SelectMany(f => f.Allergens).Distinct().Count();

            while (knownAllergens.Count != allergensCount)
            {
                var allergens = foodItems.SelectMany(f => f.Allergens).Distinct().Except(knownAllergens.Keys);
                foreach (var allergen in allergens)
                {
                    var possibleIngredients = foodItems.Where(food => food.Allergens.Any(a => a == allergen))
                                                       .Select(food => food.Ingredients.Except(knownAllergens.Values))
                                                       .Aggregate((foodA, foodB) => foodA.Intersect(foodB));

                    if (possibleIngredients.Count() is 1)
                    {
                        var ingredient = possibleIngredients.Single();
                        knownAllergens.Add(allergen, ingredient);
                        safeIngredients.Remove(ingredient);
                    }
                }
            }

            return (knownAllergens, safeIngredients);
        }

        private static Regex _regex = new Regex(@"^(?<ingredients>[^(]+)(?:\(contains (?<allergens>.*)\))?", RegexOptions.Compiled);
        private static Food ParseFood(string input)
        {
            var groups = _regex.Match(input).Groups;

            return new Food
            {
                Ingredients = groups["ingredients"].Value.Trim().Split(" ").Select(s => s.Trim()).ToList(),
                Allergens = groups["allergens"].Value.Trim().Split(",").Select(s => s.Trim()).ToList()
            };
        }

        public class Food
        {
            public List<string> Ingredients { get; set; }
            public List<string> Allergens { get; set; }
        }
    }
}