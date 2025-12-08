using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Playground;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var junctionBoxes = ParseInput(input);
        var distances = GetDistances(junctionBoxes);

        var circuts = new List<HashSet<Point>>();

        foreach (var p in junctionBoxes)
        {
            circuts.Add(new HashSet<Point>() { p });
        }

        var iterations = 0;
        while (circuts.Count > 1)
        {
            iterations++;
            var minDistance = distances.Dequeue();
            var c1 = circuts.Where(c => c.Contains(minDistance.Item1)).FirstOrDefault();
            var c2 = circuts.Where(c => c.Contains(minDistance.Item2)).FirstOrDefault();

            if (c1 == null || c2 == null) throw new Exception("Bad Data!");

            if (c1 == c2) continue;

            circuts.Remove(c2);
            for (int n = 0; n < c2.Count; n++)
            {
                c1.Add(c2.ElementAt(n));
            }

            if (circuts.Count == 1)
            {
                Console.WriteLine("Part 2: " + minDistance.Item1.X * minDistance.Item2.X);
                break;
            }

            if (iterations == 1000)
            {
                var top3 = circuts.Select(c => c.Count).OrderDescending().Take(3).Aggregate((c, n) => c * n);
                Console.WriteLine("Part 1: " + top3);
            }
        }
    }

    static List<Point> ParseInput(string[] input)
    {
        var junctionBoxes = new List<Point>();
        foreach (var line in input)
        {
            var parts = line.Split(',');
            junctionBoxes.Add(new Point(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2])
            ));
        }
        return junctionBoxes;
    }

    static PriorityQueue<Tuple<Point, Point>, double> GetDistances(List<Point> junctionBoxes)
    {
        var distances = new PriorityQueue<Tuple<Point, Point>, double>();
        for (int i = 0; i < junctionBoxes.Count - 1; i++)
        {
            for (int n = i + 1; n < junctionBoxes.Count; n++)
            {
                var distance = CalculateEuclideanDistance(junctionBoxes[i], junctionBoxes[n]);
                distances.Enqueue(new Tuple<Point, Point>(junctionBoxes[i], junctionBoxes[n]), distance);
            }
        }
        return distances;
    }

    public static double CalculateEuclideanDistance(Point box1, Point box2)
    {
        // Calculate the difference in each coordinate
        double deltaX = box2.X - box1.X;
        double deltaY = box2.Y - box1.Y;
        double deltaZ = box2.Z - box1.Z;

        // Square the differences and sum them
        double sumOfSquaredDifferences = (deltaX * deltaX) + (deltaY * deltaY) + (deltaZ * deltaZ);

        // Take the square root of the sum
        return Math.Sqrt(sumOfSquaredDifferences);
    }
}

record Point(int X, int Y, int Z);
