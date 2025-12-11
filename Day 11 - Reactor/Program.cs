using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reactor;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var outputs = ParseInput(input);
        var validPaths = CountPaths("you", "out", outputs);
        Console.WriteLine("Part 1: " + validPaths);
        PATH_MEM.Clear();
        var svrToFft = CountPaths("svr", "fft", outputs, "dac");
        PATH_MEM.Clear();
        var FftToDac = CountPaths("fft", "dac", outputs);
        PATH_MEM.Clear();
        var dacToOuput = CountPaths("dac", "out", outputs);
        Console.WriteLine("Part 2: " + svrToFft * FftToDac * dacToOuput);
    }

    static Dictionary<string, long> PATH_MEM = new Dictionary<string, long>();
    static long CountPaths(string start, string end, Dictionary<string, string[]> paths, string avoid = "")
    {
        if(start == end) return 1;
        if(start == avoid) return 0;
        if(PATH_MEM.TryGetValue(start, out long val))
        {
            return val;
        }
        var validPaths = 0L;
        if(!paths.ContainsKey(start)) return 0;
        foreach(var next in paths[start])
        {
            validPaths+= CountPaths(next, end, paths, avoid);
        }
        PATH_MEM.Add(start, validPaths);
        return validPaths;
    }

    static int CountPathsSvr(string start, string end, Dictionary<string, string[]> paths, bool dac, bool fft)
    {
        if(start == "fft") fft = true;
        if(start == "dac") dac = true;
        var validPaths = 0;
        foreach(var next in paths[start])
        {
            if(next == end)
            {
                if(fft && dac)
                    validPaths++;
                continue;
            }

            validPaths+= CountPathsSvr(next, end, paths, dac, fft);
        }
        return validPaths;
    }

    static Dictionary<string, string[]> ParseInput(string[] input)
    {
        Dictionary<string, string[]> outputs = new Dictionary<string, string[]>();
        foreach(var line in input)
        {
            var parts1 = line.Split(":");
            var values = parts1[1].Trim().Split(" ");
            outputs.Add(parts1[0], values);
        }
        return outputs;
    }
}
