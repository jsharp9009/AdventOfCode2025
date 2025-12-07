using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Laboratories;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        bool[] lasers = new bool[input.First().Length];

        var totalSplits = 0;
        lasers[input.First().IndexOf('S')] = true;
        for(int n = 2; n < input.Length; n+=2)
        {
            var row = input[n];
            for(int i = 0; i < row.Length; i++)
            {
                if(row[i] == '^' && lasers[i])
                {
                    lasers[i] = false;
                    lasers[i+1] = true;
                    lasers[i-1] = true;
                    totalSplits += 1;
                }
            }
        }
        Console.WriteLine("Part 1: " + totalSplits);
        Console.WriteLine("Part 2: " + GetPossibleWorlds(input, new Point(input.First().IndexOf('S'), 0)));
    }

    static Dictionary<Point, long> mem = new Dictionary<Point, long>();
    static long GetPossibleWorlds(string[] input, Point ray)
    {
        if(mem.TryGetValue(ray, out long worlds)) return worlds;
        
        if(input.Length == ray.y + 1) return 1;

        if(input[ray.y + 1][ray.x] == '^')
        {
            mem.Add(ray, GetPossibleWorlds(input, new Point(ray.x + 1, ray.y + 1))
                + GetPossibleWorlds(input, new Point(ray.x - 1, ray.y + 1)));
        }
        else
        {
            mem.Add(ray, GetPossibleWorlds(input, new Point(ray.x, ray.y + 1)));
        }

        return mem[ray];
    }
}

record Point(int x, int y);