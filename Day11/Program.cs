using System.Numerics;

namespace Day11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var n = (lines.Length + 1) / 7;

            var items = new List<Queue<BigInteger>>();
            var tests = new List<int>();
            var throwTo = new List<(int, int)>();
            var operations = new List<Func<BigInteger, BigInteger>>();

            for (int i = 0; i < n; i++)
            {
                items.Add(new Queue<BigInteger>(lines[i * 7 + 1].Substring(17).Split(',').Select(BigInteger.Parse)));
                tests.Add(int.Parse(lines[i * 7 + 3].Split(' ').Last()));
                throwTo.Add((   int.Parse(lines[i * 7 + 4].Split(' ').Last()),
                                int.Parse(lines[i * 7 + 5].Split(' ').Last())));

                var operation = lines[i * 7 + 2].Split(' ').Reverse().Take(3).ToArray();
                if (operation[0] == "old")
                {
                    operations.Add(x => x * x);
                }
                else if (operation[1] == "+")
                {
                    operations.Add(x => x + BigInteger.Parse(operation[0]));
                }
                else
                {
                    operations.Add(x => x * BigInteger.Parse(operation[0]));
                }
            }
            
            var inspectionCount = items.Select(_ => 0).ToList();

            for (int round = 1; round <= 10000; round++)
            {
                for (int m = 0; m < n; m++)
                {
                    while (items[m].Any())
                    {
                        var item = items[m].Dequeue();
                        inspectionCount[m]++;
                        var newWorry = operations[m](item);
                        newWorry = newWorry % 9699690;
                        //newWorry = newWorry % 96577; 
                        //newWorry = newWorry / 3;
                        if (newWorry % tests[m] == 0)
                        {
                            items[throwTo[m].Item1].Enqueue(newWorry);
                        }
                        else
                        {
                            items[throwTo[m].Item2].Enqueue(newWorry);
                        }

                    }
                }
            }

            var ordered = inspectionCount.OrderByDescending(i => i).Take(2).ToArray();
            var part1 = (long) ordered[0] * (long) ordered[1];
        }
    }
}