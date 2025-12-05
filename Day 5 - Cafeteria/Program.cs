using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cafeteria;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        (var ranges, var tests) = ParseInput(input);

        var orderedRanges = ranges.OrderBy(c => c.start).ToList();
        var combined = CombineRanges(orderedRanges);

        var goodFood = 0;
        foreach(var test in tests)
        {
            if(combined.Any(r => r.InRange(test)))
                goodFood++;
        }

        Console.WriteLine("Part 1: " + goodFood);
        Console.WriteLine("Part 2: " + combined.Sum(c => c.GetNumberOfValid()));
    }

    static List<Range> CombineRanges(List<Range> ranges)
    {
        var newRanges = new List<Range>();
        for(int i = 0; i < ranges.Count ; i++)
        {
            if(i == ranges.Count - 1)
            {
                newRanges.Add(ranges[i]);
                break;
            }
            if (ranges[i].Overlap(ranges[i + 1]))
            {
                newRanges.Add(ranges[i].Combine(ranges[i+1]));
                i++;
            }
            else
            {
                newRanges.Add(ranges[i]);
            }
        }

        if(newRanges.Count == ranges.Count) return newRanges;
        else return CombineRanges(newRanges);
    }

    static (List<Range> ranges, List<long> tests) ParseInput(string[] input)
    {
        List<Range> ranges = new List<Range>();
        List<long> tests = new List<long>();
        bool isRange = true;
        foreach(var line in input)
        {
            if (string.IsNullOrEmpty(line))
            {
                isRange = false;
                continue;
            }
            if (isRange)
            {
                var parts = line.Split('-');
                ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
            }
            else
            {
                tests.Add(long.Parse(line));
            }
        }

        return (ranges, tests);
    }
}

record Range(long start, long end)
{
    public bool InRange(long test)
    {
        return start <= test && test <= end;
    }

    public long GetNumberOfValid()
    {
        return end - start + 1;
    }

    public bool Overlap(Range other)=> start <= other.end && other.start <= end;
    
    public Range Combine(Range other) => new Range(Math.Min(start, other.start), Math.Max(end, other.end));
}
