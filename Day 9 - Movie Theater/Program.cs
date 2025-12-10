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

        (tiles, squareAreas) = CompressTiles(tiles, squareAreas);

        var maxArea = FindLargestSquarePart2(tiles, squareAreas);
        Console.WriteLine("Part 2: " + maxArea);
    }

    //4621384368 too high

    static (Point[] compressTiles, Dictionary<Tuple<Point, Point>, long>) CompressTiles(Point[] tiles, Dictionary<Tuple<Point, Point>, long> squares)
    {
        var xMap = tiles.Select(t => t.X).Distinct().OrderBy(x => x).Select((v, i) => new { Value = v, Index = i }).ToDictionary(c => c.Value, c => c.Index);
        var yMap = tiles.Select(t => t.Y).Distinct().OrderBy(y => y).Select((v, i) => new { Value = v, Index = i }).ToDictionary(c => c.Value, c => c.Index); ;

        var newTiles = tiles.Select(t => new Point(xMap[t.X], yMap[t.Y])).ToArray();
        var newSquares = squares.ToDictionary(c => new Tuple<Point, Point>(new Point(xMap[c.Key.Item1.X], yMap[c.Key.Item1.Y]), new Point(xMap[c.Key.Item2.X], yMap[c.Key.Item2.Y])), c => c.Value);

        return (newTiles, newSquares);
    }

    static Dictionary<Tuple<Point, Point>, long> FindLargestSquarePart1(Point[] tiles)
    {
        Dictionary<Tuple<Point, Point>, long> squares = new Dictionary<Tuple<Point, Point>, long>();
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
                if(edges.Contains(new Point(x, minY + 1)) || edges.Contains(new Point(x, minY - 1)))
                    return false;
            }
            for(int y = minY + 1; y < maxY; y++)
            {
                if(edges.Contains(new Point(y, minX + 1)) || edges.Contains(new Point(y, minX - 1)))
                    return false;
            }
        }
        return true;
    }

    static Dictionary<Point, bool> ColoredMem = new Dictionary<Point, bool>();
    static bool IsColoredSquare(Point[] tiles, Point toCheck, int xBoundmin, int xBoundmax, int yboundmin, int yboundmax)
    {
        if (ColoredMem.TryGetValue(toCheck, out bool value))
        {
            return value;
        }

        var between = 0;
        var tilesHit = 0;
        for (int i = toCheck.X - 1; i >= 0; i--)
        {
            var newCheck = new Point(i, toCheck.Y);
            var tileIndex = Array.IndexOf(tiles, newCheck);
            if (tileIndex >= 0)
            {
                if (newCheck.Y == yboundmin || newCheck.Y == yboundmax)
                {
                    ColoredMem.Add(toCheck, false);
                    return false;
                }

                between++;
                var nextTile = tiles[(tileIndex + 1) % tiles.Length];



                if (nextTile.Y == toCheck.Y && i > nextTile.X)
                    i = nextTile.X;
                var prevTile = tiles[(tileIndex - 1) % tiles.Length];
                if (prevTile.Y == toCheck.Y && i > prevTile.X)
                    i = prevTile.X;

            }
            else if (BetweenTwoPoints(tiles, new Point(i, toCheck.Y)))
            {
                between++;
            }

        }
        if (between % 2 != 0 || (toCheck.X == xBoundmin && BetweenTwoPoints(tiles, toCheck)))
        {
            ColoredMem.Add(toCheck, true);
            return true;
        }

        ColoredMem.Add(toCheck, false);
        return false;
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

    static Dictionary<Point, bool> BetweenMem = new Dictionary<Point, bool>();
    static bool BetweenTwoPoints(Point[] tiles, Point between)
    {
        if (BetweenMem.TryGetValue(between, out bool value))
        {
            return value;
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            var t1 = tiles[i];
            var t2 = tiles[(i + 1) % tiles.Length];

            if (t1.X != between.X || t2.X != between.X) continue;

            var maxY = Math.Max(t1.Y, t2.Y);
            var minY = Math.Min(t1.Y, t2.Y);

            if (between.Y >= minY && between.Y <= maxY)
            {
                BetweenMem.Add(between, true);
                return true;
            }
        }

        BetweenMem.Add(between, false);
        return false;
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
