using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GiftShop;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt")[0];
        var ranges = input.Split(',');

        var totalPart1 = 0L;
        var totalPart2 = 0L;
        foreach(var range in ranges)
        {
            totalPart1 += FindBadValuesPart1(range);
            totalPart2 += FindBadValuesPart2(range);
        }

        Console.WriteLine($"Part 1: {totalPart1}");
        Console.WriteLine($"Part 2: {totalPart2}");
    }

    static long FindBadValuesPart1(string range)
    {
        var total = 0L;
        var split = range.Split('-');
        if(split[0].Length % 2 == 1 && split[0].Length == split[1].Length) return 0;
        var num1 = long.Parse(split[0]);
        var num2 = long.Parse(split[1]);

        for(long i = num1; i <= num2; i++)
        {
            var numstring = i.ToString();
            if(numstring.Length % 2 == 1) continue;
            
            var middle = numstring.Length / 2;

            if (numstring[..middle].Equals(numstring[middle..]))
            {
                total += i;
            }
        }
        return total;
    }

    static long FindBadValuesPart2(string range)
    {
        var total = 0L;
        var split = range.Split('-');
        var num1 = long.Parse(split[0]);
        var num2 = long.Parse(split[1]);

        for(long i = num1; i <= num2; i++)
        {
            var numstring = i.ToString();
            var middle = numstring.Length / 2;

            for(int n = 1; n <= middle; n++)
            {
                var regex = $"^({numstring[..n]})*$";
                if(Regex.IsMatch(numstring, regex)) {
                    total += i;
                    break;
                }
            }
        }
        return total;
    }
}
