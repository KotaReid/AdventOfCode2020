using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Days
{
    public static class Day18
    {
        public static void Run()
        {
            var equations = Utils.ReadFromFile("Day18.txt").Select(ParseEquation);
            Console.WriteLine($"Part 1: {Part1(equations)}");
            Console.WriteLine($"Part 2: {Part2(equations)}");
        }

        private static long Part1(IEnumerable<IEnumerable<char>> equations) => equations.Select(equation => equation.SolveEquation(checkPrecedence: false)).Sum();
        private static long Part2(IEnumerable<IEnumerable<char>> equations) => equations.Select(equation => equation.SolveEquation(checkPrecedence: true)).Sum();

        private static long SolveEquation(this IEnumerable<char> equation, bool checkPrecedence)
        {
            var valueStack = new Stack<long>();
            var operatorStack = new Stack<char>();

            foreach (var c in equation)
            {
                if (char.IsDigit(c))
                    valueStack.Push((long)char.GetNumericValue(c));
                else if (c == '(')
                    operatorStack.Push(c);
                else if (c == ')')
                {
                    PerformExpressionWhile(() => operatorStack.Peek() != '(');
                    operatorStack.Pop();
                }
                else
                {
                    PerformExpressionWhile(() => operatorStack.Count > 0 && c.HasHigherPrecedence(operatorStack.Peek(), checkPrecedence));
                    operatorStack.Push(c);
                }
            }

            PerformExpressionWhile(() => operatorStack.Count > 0);

            return valueStack.Pop();

            void PerformExpressionWhile(Func<bool> condition)
            {
                while (condition())
                {
                    var value1 = valueStack.Pop();
                    var value2 = valueStack.Pop();
                    var operatorChar = operatorStack.Pop();

                    valueStack.Push(EvaluateExpression(value1, operatorChar, value2));
                }
            }
        }

        private static bool HasHigherPrecedence(this char currentOperator, char stackOperator, bool checkPrecedence)
            => (currentOperator, stackOperator, checkPrecedence) switch
            {
                ('+', '*', true) => false,
                (_, '(' or ')', _) => false,
                _ => true,
            };

        private static long EvaluateExpression(long value1, char operatorChar, long value2) => operatorChar switch
        {
            '+' => value1 + value2,
            '*' => value1 * value2,
            _ => throw new ArgumentException("Operator not supported")
        };

        private static IEnumerable<char> ParseEquation(string input) => input.Split(' ').SelectMany(s => s.Select(c => c));
    }
}