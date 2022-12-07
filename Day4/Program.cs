using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System;

namespace Day4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            var ranges = lines.Select(line => line.Split(','))
                .Select(
                    arr => (arr[0].Split('-').Select(int.Parse).ToArray(), 
                    arr[1].Split('-').Select(int.Parse).ToArray()))
                .ToArray();

            var part1 = ranges.Count(tp => tp.Item1[0] <= tp.Item2[0] && tp.Item1[1] >= tp.Item2[1]
                || tp.Item2[0] <= tp.Item1[0] && tp.Item2[1] >= tp.Item1[1]);

            var part2 = ranges.Count(tp => tp.Item1[0] <= tp.Item2[1] && tp.Item1[1] >= tp.Item2[0]
                || tp.Item2[1] <= tp.Item1[0] && tp.Item2[1] >= tp.Item1[0]);
        }
    }
}