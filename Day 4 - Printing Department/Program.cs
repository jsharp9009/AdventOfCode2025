using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PrintingDepartment;

class Program
{
    static List<Coord> directions = new List<Coord>()
    {
        new(0, 1),
        new(1, 1),
        new(1, 0),
        new(-1, 0),
        new(-1, 1),
        new(0, -1),
        new(-1, -1),
        new(1, -1),
    };

    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var coords = ParseInput(input);

        var remove = new HashSet<Coord>();

        var lastRemoved = 0;
        var part1Solved = false;
        
        while (lastRemoved > 0 || !part1Solved)
        {
            var removeThisRound = RemoveRolls(coords, remove);
            foreach(var r in removeThisRound)
            {
                remove.Add(r);
            }
            if (!part1Solved)
            {
                Console.WriteLine("Part 1: " + remove.Count());
                part1Solved = true;
            }
            lastRemoved = removeThisRound.Count;
        }

        Console.WriteLine("Part 2: " + remove.Count());
    }

    static HashSet<Coord> RemoveRolls(HashSet<Coord> coords, HashSet<Coord> remove)
    {
        HashSet<Coord> removeThisRound = new HashSet<Coord>();
            foreach (var coord in coords)
            {
                if(remove.Contains(coord)) continue;
                var neighbors = 0;
                foreach (var dir in directions)
                {
                    var s = coord + dir;
                    if (!remove.Contains(s) && coords.Contains(s))
                    {
                        neighbors++;
                    }

                    if (neighbors >= 4) break;
                }
                if (neighbors < 4) removeThisRound.Add(coord);
            }
            return removeThisRound;
    }

    static HashSet<Coord> ParseInput(string[] input)
    {
        var coords = new HashSet<Coord>();
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '@')
                {
                    coords.Add(new Coord(x, y));
                }
            }
        }
        return coords;
    }
}

record Coord(int x, int y)
{
    public static Coord operator +(Coord c1, Coord c2)
    {
        return new Coord(c1.x + c2.x, c1.y + c2.y);
    }
}
