using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Runtime.Intrinsics.Arm;

namespace Day18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var points = new HashSet<(int x, int y, int z)>(File.ReadAllLines("input.txt").Select(str => str.Split(',')).Select(arr => (int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]))).ToArray());

            var offsets = new (int x, int y, int z)[] { (-1, 0, 0), (1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1) };

            int area = 0;
            foreach (var point in points)
            {
                var notConnectedCount = offsets.Select(offset => (point.x + offset.x, point.y + offset.y, point.z + offset.z)).Count(p2 => !points.Contains(p2));
                area += notConnectedCount;
            }
        }

    }
}