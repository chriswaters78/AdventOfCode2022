using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Drawing;
using System.Xml.Linq;

namespace Day18
{
    internal class Program
    {
        static (int x, int y, int z)[] _OFFSETS_ = new [] { (-1, 0, 0), (1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };
        static int minx, miny, minz, maxx, maxy, maxz;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var cubes = new HashSet<(int x, int y, int z)>(File.ReadAllLines($"{args[0]}.txt")
                .Select(str => str.Split(','))
                .Select(arr => (int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]))).ToArray());

            var part1 = cubes.Aggregate(0, (area, point) => area + 
                offsets(point).Count(p2 => 
                    !cubes.Contains(p2)));

            Console.WriteLine($"Part 1: {part1} in {watch.ElapsedMilliseconds}ms");

            //these limits ensure there is a box around the droplets that is all air
            (minx, maxx) = (cubes.Min(p => p.x) - 1, cubes.Max(p => p.x) + 1);
            (miny, maxy) = (cubes.Min(p => p.y) - 1, cubes.Max(p => p.y) + 1);
            (minz, maxz) = (cubes.Min(p => p.z) - 1, cubes.Max(p => p.z) + 1);

            //find all points connected to the exterior
            var exteriorPoints = connected(cubes, (minx, miny, minz));

            var part2 = cubes.Aggregate(0, (area, point) => area + 
                offsets(point).Count(p2 => 
                    !cubes.Contains(p2) 
                    && exteriorPoints.Contains(p2)));
            

            Console.WriteLine($"Part 2: {part2} in {watch.ElapsedMilliseconds}ms");
        }

        private static IEnumerable<(int x, int y, int z)> offsets((int x, int y, int z) point)
        {
            return _OFFSETS_.Select(offset => (point.x + offset.x, point.y + offset.y, point.z + offset.z));
        }

        private static HashSet<(int x, int y, int z)> connected(HashSet<(int x, int y, int z)> cubes, (int x, int y, int z) root)
        {
            var visited = new HashSet<(int x, int y, int z)>();
            var queue = new Queue<(int x, int y, int z)>();
            queue.Enqueue(root);

            visited.Add(root);
            while (queue.Any())
            {
                var node = queue.Dequeue();
                foreach ((int x, int y, int z) point in offsets(node))
                {
                    if (point.x >= minx && point.x <= maxx
                        && point.y >= miny && point.y <= maxy
                        && point.z >= minz && point.z <= maxz
                        && !visited.Contains(point) 
                        && !cubes.Contains(point))
                    {
                        visited.Add(point);
                        queue.Enqueue(point);
                    }
                }
            }

            return visited;
        }
    }
}