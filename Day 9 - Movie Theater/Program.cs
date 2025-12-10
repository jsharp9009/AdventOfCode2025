using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MovieTheater;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var tiles = ParseInput(input);
        var squareAreas = FindLargestSquarePart1(tiles);
        squareAreas = squareAreas.OrderByDescending(c => c.Value).ToDictionary();
        Console.WriteLine("Part 1: " + squareAreas.ElementAt(0).Value);
        //Only compress if were massive. Otherwise, it doesn't work.
        if(tiles.Max(c => c.X) > 1000)
            (tiles, squareAreas) = CompressTiles(tiles, squareAreas);

        var maxArea = FindLargestSquarePart2(tiles, squareAreas);
        Console.WriteLine("Part 2: " + maxArea);
    }

    static (Point[] compressTiles, Dictionary<Tuple<Point, Point>, long>) CompressTiles(Point[] tiles, Dictionary<Tuple<Point, Point>, long> squares)
    {
        var uniqueX = tiles.Select(t => t.X).Distinct().OrderBy(x => x);
        var uniqueY = tiles.Select(t => t.Y).Distinct().OrderBy(y => y);

        var xMap = uniqueX.Select((v, i)=> new{Value = v, index = i}).ToDictionary(c => c.Value, c=> c.index);
        var yMap = uniqueY.Select((v, i)=> new{Value = v, index = i}).ToDictionary(c => c.Value, c=> c.index);

        var newTiles = tiles.Select(t => new Point(xMap[t.X], yMap[t.Y])).ToArray();
        var newSquares = squares.ToDictionary(c => new Tuple<Point, Point>(new Point(xMap[c.Key.Item1.X], yMap[c.Key.Item1.Y]), new Point(xMap[c.Key.Item2.X], yMap[c.Key.Item2.Y])), c => c.Value);

        return (newTiles, newSquares);
    }

    static Dictionary<Tuple<Point, Point>, long> FindLargestSquarePart1(Point[] tiles)
    {
        Dictionary<Tuple<Point, Point>, long> squares = [];
        for (int i = 0; i < tiles.Length - 1; i++)
        {
            for (int n = 1; n < tiles.Length; n++)
            {
                var p1 = tiles[i];
                var p2 = tiles[n];

                var area = (Math.Abs(p1.X - p2.X) + 1L) * (Math.Abs(p1.Y - p2.Y) + 1L);
                squares.Add(new Tuple<Point, Point>(p1, p2), area);
            }
        }
        return squares;
    }

    static long FindLargestSquarePart2(Point[] tiles, Dictionary<Tuple<Point, Point>, long> squares)
    {
        var edges = GetEdges(tiles);

        foreach (var square in squares)
        {
            var p1 = square.Key.Item1;
            var p2 = square.Key.Item2;

            var testPoint1 = new Point(p1.X, p2.Y);
            var testPoint2 = new Point(p2.X, p1.Y);

            if(!TestSquare([p1, p2], edges)) continue;

            return square.Value;
            // var area = (Math.Abs(p1.X - p2.X) + 1L) * (Math.Abs(p1.Y - p2.Y) + 1L);
            // return area;
        }
        return 0L;
    }

    static bool TestSquare(Point[] square, HashSet<Point> edges)
    {
        for (int i = 0; i < square.Length; i++)
        {
            var t1 = square[i];
            var t2 = square[(i + 1) % square.Length];

            var minY = Math.Min(t1.Y, t2.Y);
            var maxY = Math.Max(t1.Y, t2.Y);
            var minX = Math.Min(t1.X, t2.X);
            var maxX = Math.Max(t1.X, t2.X);

            for(int x = minX + 1; x < maxX; x++)
            {
                if(edges.Contains(new Point(x, minY + 1)) || edges.Contains(new Point(x, maxY - 1)))
                    return false;
            }
            for(int y = minY + 1; y < maxY; y++)
            {
                if(edges.Contains(new Point(minX + 1, y)) || edges.Contains(new Point(maxX - 1, y)))
                    return false;
            }
        }
        return true;
    }

    static HashSet<Point> GetEdges(Point[] tiles)
    {
        HashSet<Point> edges = new HashSet<Point>();
        for (int i = 0; i < tiles.Length; i++)
        {
            var t1 = tiles[i];
            var t2 = tiles[(i + 1) % tiles.Length];
            if (t1.X == t2.X)
            {
                var minY = Math.Min(t1.Y, t2.Y);
                var maxY = Math.Max(t1.Y, t2.Y);

                for (int y = minY; y <= maxY; y++)
                {
                    edges.Add(new Point(t1.X, y));
                }
            }
            else if (t1.Y == t2.Y)
            {
                var minX = Math.Min(t1.X, t2.X);
                var maxX = Math.Max(t1.X, t2.X);

                for (int x = minX; x <= maxX; x++)
                {
                    edges.Add(new Point(x, t1.Y));
                }
            }
        }
        return edges;
    }

    static Point[] ParseInput(string[] input)
    {
        var tiles = new List<Point>();
        foreach (var line in input)
        {
            var parts = line.Split(",");
            tiles.Add(new(int.Parse(parts[0]), int.Parse(parts[1])));
        }
        return [.. tiles];
    }
}

record Point(int X, int Y);
