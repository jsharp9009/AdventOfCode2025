using System;
using System.IO;
using System.Linq;

namespace SecretEntrance;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");

        var lockNum = 50;
        var newLockNum = 50;
        var zeroCountPart1 = 0;
        var zeroCountPart2 = 0;
        foreach(var line in input)
        {
            var num = int.Parse(line[1..]);
            if(num > 100)
            {
                zeroCountPart2 += num / 100;
                num %= 100;
            }
            if (line[0] == 'R')
            {
                newLockNum += num;
                if(newLockNum >= 100)
                {
                    zeroCountPart2++;
                }
            }
            else
            {
                newLockNum -= num;
                if(lockNum > 0 && newLockNum <= 0){
                    zeroCountPart2 ++;
                }
            }

           
            if(newLockNum >= 100)
            {
                newLockNum %= 100;
            }
            else if(newLockNum < 0)
            {
                while(newLockNum < 0)
                {
                    newLockNum += 100;
                }
            }

            if(newLockNum == 0) zeroCountPart1++;
            //Console.WriteLine($"lock: {newLockNum}, zeroCount: {zeroCountPart2}");
            lockNum = newLockNum;
        }

        Console.WriteLine($"Part 1: {zeroCountPart1}");
        Console.WriteLine($"Part 2: {zeroCountPart2}");
    }
}


//5487 too low