using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Drawing;

namespace Day18
{
    internal class Program
    {
        static (int x, int y, int z)[] offsets = new [] { (-1, 0, 0), (1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };

        static int minx;
        static int miny;
        static int minz;
        static int maxx;
        static int maxy;
        static int maxz;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Console.WriteLine($"Part 1: {part1()} in {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part 2: {part2()} in {watch.ElapsedMilliseconds}ms");
        }
        private static int part2()
        {
            var cubes = new HashSet<(int x, int y, int z)>(File.ReadAllLines("input.txt").Select(str => str.Split(',')).Select(arr => (int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]))).ToArray());

            //these limits ensure there is a box around the droplets that is all air
            (minx, maxx) = (cubes.Min(p => p.x) - 1, cubes.Max(p => p.x) + 1);
            (miny, maxy) = (cubes.Min(p => p.y) - 1, cubes.Max(p => p.y) + 1);
            (minz, maxz) = (cubes.Min(p => p.z) - 1, cubes.Max(p => p.z) + 1);

            HashSet<(int x, int y, int z)> exteriorPoints = connected(cubes, new HashSet<(int x, int y, int z)>(), (minx, miny, minz));

            int area = 0;
            
            foreach (var point in cubes)
            {
                var exteriorCount = offsets.Select(offset => (point.x + offset.x, point.y + offset.y, point.z + offset.z))
                    .Count(p2 => !cubes.Contains(p2) && exteriorPoints.Contains(p2));
                area += exteriorCount;
            }

            return area;
        }

        private static HashSet<(int x, int y, int z)> connected(HashSet<(int x, int y, int z)> cubes, HashSet<(int x, int y, int z)> visited, (int x, int y, int z) root)
        {
            foreach ((int x, int y, int z) point in offsets.Select(offset => (root.x + offset.x, root.y + offset.y, root.z + offset.z)))
            {
                if (point.x >= minx && point.x <= maxx
                    && point.y >= miny && point.y <= maxy
                    && point.z >= minz && point.z <= maxz
                    && !visited.Contains(point) && !cubes.Contains(point))
                {
                    visited.Add(point);
                    connected(cubes, visited, point);
                }
            }

            return visited;
        }
            

        private static int part1()
        {
            var points = new HashSet<(int x, int y, int z)>(File.ReadAllLines("input.txt").Select(str => str.Split(',')).Select(arr => (int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]))).ToArray());

            int area = 0;
            foreach (var point in points)
            {
                var notConnectedCount = offsets.Select(offset => (point.x + offset.x, point.y + offset.y, point.z + offset.z)).Count(p2 => !points.Contains(p2));
                area += notConnectedCount;
            }

            return area;
        }

    }
}