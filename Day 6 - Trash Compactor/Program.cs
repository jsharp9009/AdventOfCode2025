using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TrashCompactor;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        (var problems, var operators) = ParseInputPart1(input);

        var total = 0L;
        for(int i = 0; i < problems.Count; i++)
        {
            total += SolveProblemPart1(problems[i], operators[i]);
        }
        Console.WriteLine("Part 1: " + total);

        (var problems2, operators) = ParseInputPart2(input);

        total = 0L;
        for(int i = 0; i < problems2.Count; i++)
        {
            total += SolveProblemPart2(problems2[i], operators[i]);
        }
        Console.WriteLine("Part 2: " + total);

    }
    
    static long SolveProblemPart2(List<string> problem, char op)
    {
        var numbers = new List<long>();
        for(int i = 0; i < problem.First().Length; i++)
        {
            var sb = new StringBuilder();
            for(int n = 0; n < problem.Count; n++)
            {
                sb.Append(problem[n][i]);
            }
            numbers.Add(long.Parse(sb.ToString().Trim()));
        }

        return SolveProblemPart1(numbers, op);
    }

    static long SolveProblemPart1(List<long> problem, char op)
    {
        if(op == '+')
        {
            return problem.Sum();
        }
        else
        {
            return problem.Aggregate(1L, (currentProduct, nextNumber) => currentProduct * nextNumber);
        }
    }

    static (List<List<long>>, char[]) ParseInputPart1(string[] input)
    {
        List<List<long>> problems = new List<List<long>>();

        var numbers = input.Take(input.Length - 1).Select(c => c.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToArray();
        for(long x = 0; x < numbers.First().Length; x++)
        {   
            List<long> problem = new List<long>();
            for(long y = 0; y < numbers.Length; y++)
            {
                problem.Add(long.Parse(numbers[y][x]));
            }
            problems.Add(problem);
        }

        var operators = input.Last().Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(c => c != " ").Select(s => s[0]).ToArray();
        return (problems, operators);
    }

    static (List<List<string>>, char[]) ParseInputPart2(string[] input)
    {
        Regex reg = new Regex("\\d+", RegexOptions.Compiled);

        var maxColumnLenghts = new Dictionary<int, int>();
        foreach(var s in input)
        {
            var matches = reg.Matches(s);
            for(int i= 0; i < matches.Count; i++)
            {   
                var m = matches[i];
                if (maxColumnLenghts.TryGetValue(i, out int value))
                {
                    maxColumnLenghts[i] = Math.Max(m.Length, value);
                }
                else
                {
                    maxColumnLenghts.Add(i, m.Length);
                }
            }
        }

        List<List<string>> problems = new List<List<string>>();
        var offset = 0;
        var numberOfProblems = input.First().Split(" ", StringSplitOptions.RemoveEmptyEntries).Count();
        for(int x = 0; x < numberOfProblems; x++)
        {   
            List<string> problem = new List<string>();
            var columnLength = maxColumnLenghts[x];
            for(int y = 0; y < input.Length - 1; y++)
            {
                problem.Add(input[y].Substring(offset, columnLength));
            }
            problems.Add(problem);
            offset += columnLength + 1;
        }

        var operators = input.Last().Split(" ", StringSplitOptions.RemoveEmptyEntries).Where(c => c != " ").Select(s => s[0]).ToArray();
        return (problems, operators);
    }
}
