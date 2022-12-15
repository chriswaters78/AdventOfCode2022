using System.Collections.Generic;
using System.Diagnostics;

namespace Day15
{
    internal class Program
    {
        //const int MAX = 20;
        const int MAX = 4000000;
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var coords = File.ReadAllLines("input.txt")
                .Select(str => str.Split(": "))
                .Select(arr => (arr[0][10..].Split(", "), arr[1][21..].Split(", ")))
                .Select(tp => (tp.Item1.Select(str => int.Parse(str[2..])).ToArray(), tp.Item2.Select(str => int.Parse(str[2..])).ToArray()))
                .Select(tp => (tp.Item1[0], tp.Item1[1], tp.Item2[0], tp.Item2[1]))
                .ToArray<(int sx, int sy, int bx, int by)>();

            var ranges = new Dictionary<int, HashSet<(int, int)>>();

            //add all beacons and sensors as impossible positions
            foreach ((int x, int y) in coords.SelectMany(tp => new[] { (tp.sx, tp.sy), (tp.bx, tp.by) }))
            {
                if (!ranges.ContainsKey(y))
                {
                    ranges[y] = new HashSet<(int, int)>();
                }
                ranges[y].Add((x, x));
            }

            foreach (var coord in coords)
            {
                var d = Math.Abs(coord.sx - coord.bx) + Math.Abs(coord.sy - coord.by);

                //for all rows it covers points in
                for (int y = coord.sy - d; y <= coord.sy + d; y++)
                {
                    var dx = d - Math.Abs(coord.sy - y);
                    var x1 = coord.sx - dx;
                    var x2 = coord.sx + dx;

                    //exclude the range [x1..x2]
                    //we have to process any ranges currently in the list
                    if (!ranges.ContainsKey(y))
                    {
                        ranges[y] = new HashSet<(int, int)>();
                    }
                    var newRanges = new HashSet<(int, int)>();
                    foreach ((int rx1, int rx2) in ranges[y])
                    {
                        if (x1 <= rx2 + 1 && x2 >= rx1 - 1)
                        {
                            //can be merged
                            (x1, x2) = (Math.Min(x1, rx1), Math.Max(x2, rx2));
                        }
                        else
                        {
                            //must be completely separate
                            newRanges.Add((rx1, rx2));
                        }
                    }

                    newRanges.Add((x1, x2));
                    ranges[y] = newRanges;
                }
            }

            //find a row with a single possibility
            var solution = ranges
                .Where(kvp => kvp.Key >= 0 && kvp.Key <= MAX && kvp.Value.Count > 1).Single();

            //wouldn't work if the single possibility was right on the edge (x = 0 or x = MAX)
            //but seems it always does :)
            var answers =
                solution.Value.SelectMany(tp => new[] { tp.Item1, tp.Item2 })
                .Where(x => x >= 0 && x <= MAX)
                .ToList();

            var part1 = ranges[2000000].Single().Item2 - ranges[2000000].Single().Item1;
            var (p2x, p2y) = (Math.Abs((answers[1] + answers[0]) / 2), solution.Key);
            var part2 = ((long)p2x) * 4000000 + p2y;

            Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Part 2: {part2}");
            Console.WriteLine($"Time taken: {watch.ElapsedMilliseconds}ms");
        }
    }
}