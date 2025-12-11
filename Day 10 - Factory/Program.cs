using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Factory;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllLines("input.txt");
        var machines = ParseInput(input);
        var presses = 0;
        foreach (var m in machines)
        {
            presses += GetFewstPressesForLights(m);
        }
        Console.WriteLine("Part 1: " + presses);

        presses = 0;
        foreach (var m in machines)
        {
            int initialMask = (1 << m.Buttons.Count) - 1;
            var p = GetFewstPressesForJoltage(m.JoltageRequirements, m.Buttons, initialMask);
            presses += p;
            Console.WriteLine(p);
        }
        Console.WriteLine("Part 2: " + presses);
    }

    static int GetFewstPressesForLights(Machine m)
    {
        Queue<Tuple<bool[], int>> lightsToPress = new Queue<Tuple<bool[], int>>();
        lightsToPress.Enqueue(new Tuple<bool[], int>(new bool[m.Lights.Length], 0));
        while (lightsToPress.TryDequeue(out Tuple<bool[], int>? light))
        {
            foreach (var b in m.Buttons)
            {
                var newLights = new bool[light.Item1.Length];
                Array.Copy(light.Item1, newLights, newLights.Length);
                foreach (var i in b)
                {
                    newLights[i] = !newLights[i];
                }

                if (Enumerable.SequenceEqual(m.Lights, newLights))
                    return light.Item2 + 1;

                lightsToPress.Enqueue(new Tuple<bool[], int>(newLights, light.Item2 + 1));
            }

        }
        return int.MaxValue;
    }

    static int GetFewstPressesForJoltage(int[] joltage, List<List<int>> buttons, int mask)
    {
        if(joltage.All(j => j == 0)) return 0;

        var nonZeroJoltages = joltage.Select((v, i) => (Value: v, Index: i))
            .Where(j => j.Value > 0)
            .ToList();

        var next = nonZeroJoltages.OrderBy(t => buttons.Select((b, i)=> (Index: i, Button: b))
        .Count(b => IsButtonAvailable(b.Index, mask) && b.Button.Contains(t.Index)))
        .ThenByDescending(t => t.Value).First();

        int countToReduce = next.Index;
        int minValue = next.Value;

        var matchingButtons = buttons.Select((b, i)=> (Index: i, Button: b))
            .Where(b => IsButtonAvailable(b.Index, mask) && b.Button.Contains(countToReduce))
            .ToArray();

        var best = int.MaxValue;
        if (matchingButtons.Any())
        {
            var newMask = mask;
            foreach(var b in matchingButtons)
            {
                newMask &= ~(1 << b.Index);
            }

            var counts = new int[matchingButtons.Length];
            counts[^1] = minValue;

            do
            {
                int[] newJ = (int[])joltage.Clone();
                var ok = true;
                for(int i = 0; i < counts.Length && ok; i++)
                {
                    var count = counts[i];
                    if(count == 0) continue;

                    foreach(var p in matchingButtons[i].Button)
                    {
                        if(newJ[p] > count)
                            newJ[p] -= count;
                        else
                        {
                            ok = false;
                            break;
                        }
                    }
                }
                if (ok)
                {
                    int r = GetFewstPressesForJoltage(newJ, buttons, newMask);
                    if(r != int.MaxValue)
                        best = Math.Min(best, minValue + r);
                }
            }while(NextCombination(counts));
        }

        return best;
    }

    static bool NextCombination(int[] combination)
    {
        var i = Array.FindLastIndex(combination, v => v != 0);
        if(i <= 0) return false;

        int v = combination[i];
        combination[i - 1] += 1;
        combination[i] = 0;
        combination[^1] = v - 1;
        return true;
    }

    static bool IsButtonAvailable(int button, int mask)
    {
        return ((mask >> button) & 1) != 0;
    }

    static int GetFewstPressesForJoltageBackTracking(Machine m)
    {
        var requirementsLength = m.JoltageRequirements.Length;
        var buttonLength = m.Buttons.Count;

        //Build a matrix of what each button presses. 
        //Each row is a different button
        //1 represents that it increments that index by 1
        var matrix = new int[requirementsLength, buttonLength];
        for (int i = 0; i < buttonLength; i++)
        {
            foreach (var index in m.Buttons[i])
            {
                matrix[index, i] = 1;
            }
        }

        var limits = new int[buttonLength];
        //Find Highest Limit
        for (int i = 0; i < buttonLength; i++)
        {
            int min = int.MaxValue;
            for (int n = 0; n < requirementsLength; n++)
            {
                if (matrix[n, i] == 1)
                {
                    min = Math.Min(min, m.JoltageRequirements[n]);
                }
            }
            limits[i] = min;
        }

        var order = Enumerable.Range(0, buttonLength).OrderByDescending(b => 
            Enumerable.Range(0, requirementsLength).Count(i => matrix[i, b] == 1))
            .ToList();

        var orderedMatrix = new int[requirementsLength, buttonLength];
        for(int i = 0; i < buttonLength; i++)
        {
            int oldOrder = order[i];
            for(int r = 0; r < requirementsLength; r++)
            {
                orderedMatrix[r, i] = matrix[r, oldOrder];
            }
        }
        matrix = orderedMatrix;

        limits = order.Select(i => limits[i]).ToArray();

        var bestTotal = int.MaxValue;
        BacktrackAnswer(0, new int[requirementsLength], new List<int>());
        return bestTotal;

        void BacktrackAnswer(int button, int[] current, List<int> toCheck)
        {
            var sum = toCheck.Sum();
            if (sum > bestTotal)
                return;

            if (button == buttonLength)
            {
                for (int i = 0; i < requirementsLength; i++)
                {
                    if (current[i] != m.JoltageRequirements[i])
                        return;
                }
                bestTotal = sum;
                return;
            }

            int localMax = limits[button];
            for(int i = 0; i < requirementsLength; i++)
            {
                if(matrix[i, button] == 1)
                {
                    int remaining = m.JoltageRequirements[i] - current[i];
                    if(remaining < localMax)
                        localMax = remaining;
                }
            }
            if (localMax < 0) return;

            for (int p = 0; p <= localMax; p++)
            {
                var over = false;
                int[] next = (int[])current.Clone();
                for (int i = 0; i < requirementsLength; i++)
                {
                    next[i] += matrix[i, button] * p;
                    if(next[i] > m.JoltageRequirements[i]){
                        over = true;
                        break;
                    }
                }
                if (over) break;

                toCheck.Add(p);
                BacktrackAnswer(button + 1, next, toCheck);
                toCheck.RemoveAt(toCheck.Count - 1);
            }
        }
    }



    static List<Machine> ParseInput(string[] input)
    {
        List<Machine> machines = new List<Machine>();

        foreach (var line in input)
        {
            var parts1 = line.Split(']');
            var parts2 = parts1[1].Split('{');

            List<bool> lights = [];
            var lightString = parts1[0];

            for (int i = 1; i < lightString.Length; i++)
            {
                lights.Add(lightString[i] == '#');
            }

            var buttons = new List<List<int>>();
            var buttonString = parts2[0].Trim();

            var numString = "";
            foreach (var b in buttonString)
            {
                if (b == ' ') continue;
                if (b == '(') buttons.Add(new List<int>());
                if (char.IsDigit(b)) numString += b;
                if (b == ',' || b == ')')
                {
                    buttons.Last().Add(int.Parse(numString));
                    numString = "";
                }
            }

            var joltages = string.Concat(parts2[1][..^1]).Split(',').Select(int.Parse).ToArray();

            machines.Add(new Machine([.. lights], buttons, joltages));
        }
        return machines;
    }
}

class Machine(bool[] lights, List<List<int>> buttons, int[] joltage)
{

    public bool[] Lights { get; set; } = lights;
    public List<List<int>> Buttons { get; set; } = buttons;
    public int[] JoltageRequirements { get; set; } = joltage;
}