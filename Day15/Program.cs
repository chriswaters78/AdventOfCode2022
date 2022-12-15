using System.Collections.Generic;

namespace Day15
{
    internal class Program
    {
        //const int MAX = 20;
        const int MAX = 4000000;
        static void Main(string[] args)
        {
            var a2 = part2();
            var a2answer = ((long) a2.Item1) * 4000000 + a2.Item2;
        }

        static (int,int) part2()
        {
            var coords = File.ReadAllLines("input.txt")
                .Select(str => str.Split(": "))
                .Select(arr => (arr[0][10..].Split(", "), arr[1][21..].Split(", ")))
                .Select(tp => (tp.Item1.Select(str => int.Parse(str[2..])).ToArray(), tp.Item2.Select(str => int.Parse(str[2..])).ToArray()))
                .Select(tp => (tp.Item1[0], tp.Item1[1], tp.Item2[0], tp.Item2[1]))
                .ToArray<(int sx, int sy, int bx, int by)>();
            
            var impossible = new Dictionary<int, HashSet<(int, int)>>();
            //add all beacons and sensors as impossible positions
            foreach ((int sx, int sy, int bx, int by) in coords)
            {
                if (!impossible.ContainsKey(by))
                {
                    impossible[by] = new HashSet<(int, int)>();
                }
                impossible[by].Add((bx, bx));
                if (!impossible.ContainsKey(sy))
                {
                    impossible[sy] = new HashSet<(int, int)>();
                }
                impossible[sy].Add((sx, sx));
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

                    //exclude the range x1 => x2
                    //we have to process any ranges currently in the list

                    if (!impossible.ContainsKey(y))
                    {
                        impossible[y] = new HashSet<(int, int)>();
                    }
                    var newRanges = new HashSet<(int, int)>();
                    foreach ((int rx1, int rx2) in impossible[y])
                    {
                        //merge these
                        if (x1 == rx2 + 1)
                        {
                            x1 = rx1;
                        }
                        else if (x2 == rx1 - 1)
                        {
                            x2 = rx2;
                        }
                        //seperate
                        else if (x2 < rx1 || x1 > rx2)
                        {
                            newRanges.Add((rx1, rx2));
                        }
                        //fully enclosed
                        else if (x1 >= rx1 && x2 <= rx2)
                        {
                            (x1, x2) = (rx1, rx2);
                        }
                        //fully encloses
                        else if (x1 <= rx1 && x2 >= rx2)
                        {                            
                        }
                        //start overlaps
                        else if (x1 < rx1)
                        {
                            x2 = rx2;
                        }
                        //end overlaps
                        else if (x2 > rx2)
                        {
                            x1 = rx1;
                        }
                        else
                        {
                            throw new Exception("BOOM!");
                        }
                    }

                    newRanges.Add((x1, x2));
                    impossible[y] = newRanges;
                }
            }

            //find a row with a single possibility
            var maybe = impossible.Where(kvp => kvp.Key >= 0 && kvp.Key <= MAX && kvp.Value.Count > 1).ToList();
            foreach (var kvp in maybe)
            {
                for (int x = 0; x <= MAX; x++)
                {
                    //check row to see if there is a single possibility
                    if (kvp.Value.All(tp => !(tp.Item1 <= x && tp.Item2 >= x)))
                    {
                        return (x, kvp.Key);
                    }
                }
            }

            return (-1, -1);
        }
        static int part1()
        {
            var coords = File.ReadAllLines("input.txt")
                .Select(str => str.Split(": "))
                .Select(arr => (arr[0][10..].Split(", "), arr[1][21..].Split(", ")))
                .Select(tp => (tp.Item1.Select(str => int.Parse(str[2..])).ToArray(), tp.Item2.Select(str => int.Parse(str[2..])).ToArray()))
                .Select(tp => (tp.Item1[0], tp.Item1[1], tp.Item2[0], tp.Item2[1]))
                .ToArray<(int sx, int sy, int bx, int by)>();
            int R = 2000000;

            HashSet<int> impossible = new HashSet<int>();
            foreach (var coord in coords)
            {
                var d = Math.Abs(coord.sx - coord.bx) + Math.Abs(coord.sy - coord.by);
                var dx = d - Math.Abs(coord.sy - R);
                var x1 = coord.sx - dx;
                var x2 = coord.sx + dx;

                for (int x = x1; x <= x2; x++)
                {
                    impossible.Add(x);
                }
            }

            foreach ((int sx, int sy, int bx, int by) in coords)
            {
                if (by == R && impossible.Contains(bx))
                {
                    impossible.Remove(bx);
                }
            }

            return impossible.Count;
        }
    }
}