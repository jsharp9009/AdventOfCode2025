using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChristmasTreeFarm;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        (List<int> presents, List<Tree> trees) = ParseInput(input);

        var fit = 0;
        foreach (var tree in trees)
        {
            if (tree.PresentsFit(presents))
                fit++;
        }
        Console.WriteLine("Part 1: " + fit);
    }

    static (List<int>, List<Tree>) ParseInput(string[] input)
    {
        bool ontoTrees = false;
        List<int> presents = new List<int>();
        List<Tree> trees = new List<Tree>();
        var currentPresentArea = 0;
        foreach (var line in input.Skip(1))
        {
            if (string.IsNullOrEmpty(line))
            {
                presents.Add(currentPresentArea);
                currentPresentArea = 0;
                continue;
            }

            if (!ontoTrees && char.IsDigit(line[0]) && line[1] != ':') ontoTrees = true;

            if (!ontoTrees)
            {
                if (char.IsDigit(line[0])) continue;
                if (line[0] == '#' || line[0] == '.')
                {
                    currentPresentArea += line.Count(c => c == '#');
                }
            }
            else
            {
                var parts1 = line.Split(":");
                var treeArea = parts1[0].Split("x");
                var PresentCounts = parts1[1].Trim().Split(" ").Select(c => int.Parse(c)).ToList();

                trees.Add(new Tree(int.Parse(treeArea[0]), int.Parse(treeArea[1]), PresentCounts));
            }
        }

        return (presents, trees);
    }
}

record Tree(int Height, int Width, List<int> PresentsCounts)
{
    public bool PresentsFit(List<int> presentAreas)
    {
        var totalPresentArea = 0;
        for (int i = 0; i < PresentsCounts.Count; i++)
        {
            totalPresentArea += PresentsCounts[i] * presentAreas[i];
        }

        return (Height * Width) > totalPresentArea;
    }
}
