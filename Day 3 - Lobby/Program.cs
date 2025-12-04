using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lobby;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        //Parse the input into tuples of <value, index>
        var parsedInput = ParseInput(input);
        Console.WriteLine("Part 1: " + GetTotalPart1(parsedInput));
        Console.WriteLine("Part 2: " + GetTotalPart2(parsedInput));
    }

    static long GetTotalPart2(List<List<Tuple<int, int>>> input)
    {
        var totalVolts = 0L;

        foreach(var line in input)
        {
            var largest = FindLargestValue(line, 12, -1);
            //Console.WriteLine(largest);
            totalVolts += largest;
        }

        return totalVolts;
    }

    static long FindLargestValue(List<Tuple<int, int>> input, int digits, int lastIndex)
    {
        if(digits == 0) return 0;

        //If we need all of the digits left, just turn them into a number, save time sorting
        if(input.Count - lastIndex - 1 == digits) {
            var total = 0L;
            for(int i = 1; i <= digits; i++)
            {
                total += input[i + lastIndex].Item1 * (long)Math.Pow(10, digits - i);
            }
            return total;
        }
        
        //largest number is going to be the largest digit that is not 12 digits from the end,
        //and the largest digit after that that is X number of digits from the end.
        var ordered = input.Where(c => c.Item2 > lastIndex && c.Item2 <= input.Count - digits)
            .OrderByDescending(c => c.Item1)
            .ThenBy(c => c.Item2);

        if(ordered.Count() == 0) return 0;

        return ((long)(ordered.First().Item1 * Math.Pow(10, digits - 1))) + FindLargestValue(input, digits - 1, ordered.First().Item2);
    }

    static long GetTotalPart1(List<List<Tuple<int, int>>> parsedInput)
    {
        var totalVolts = 0;
        foreach(var p in parsedInput)
        {
            //largest number is going to be the largest digit that is not the last digit,
            //and the largest digit after that.
            var ordered = p.OrderByDescending(c => c.Item1).ThenBy(c => c.Item2);
            foreach(var t in ordered)
            {
                var largest = t;
                //Order by Value then by Index
                //Index so I get the FIRST isntance of a digit.
                var next = ordered.Where(x => x.Item2 > t.Item2).OrderByDescending(c => c.Item1);
                if (next.Any())
                {
                    totalVolts += (t.Item1 * 10) + next.First().Item1;
                    break;
                }
            }
        }
        return totalVolts;
    }

    static List<List<Tuple<int, int>>> ParseInput(string[] input)
    {
        var parsed = new List<List<Tuple<int, int>>>();
        foreach (var line in input)
        {
            var l = new List<Tuple<int, int>>();
            for (int i = 0; i < line.Length; i++)
            {
                l.Add(new Tuple<int, int>(int.Parse(line[i].ToString()), i));
            }
            parsed.Add(l);
        }
        return parsed;
    }
}
