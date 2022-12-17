using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;

namespace Day17
{
    internal class Program
    {
        //####

        //.#.
        //###
        //.#.

        //..#
        //..#
        //###

        //#
        //#
        //#
        //#

        //##
        //##

        static (int ox, int oy)[][] shapes = new[] {
                new [] {(0,0), (1,0), (2,0), (3,0) },
                new [] {(0,1), (1,0), (1,1), (1,2), (2,1) },
                new [] {(0,0), (1,0), (2,0), (2,1), (2,2)},
                new [] {(0,0), (0,1), (0,2), (0,3)},
                new [] {(0,0), (0,1), (1,0), (1,1)},
            };

        static HashSet<(int x, int y)> rocks = new HashSet<(int x, int y)>();

        const int MAXR = 10000;
        const int MAXY = 2*MAXR;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            var jets = File.ReadAllText($"{args[0]}.txt").Select(ch => ch).ToArray();

            for (int x = 0; x < 9; x++)
            {
                rocks.Add((x, 0));
            }
            for (int y = 0; y < MAXY; y++)
            {
                rocks.Add((0, y));
                rocks.Add((8, y));
            }

            //find the leftover from when the pattern repeates
            ulong ROCKS = 1000000000000;
            ROCKS -= 1276;

            ulong div = ROCKS / 1735;
            ulong rem = ROCKS % 1735;

            //then answer = 1912 + div * 2711 + tallestRock

            int tallestRock = 0;

            List<(int tallestRock, int r, int s)> repeats = new List<(int tallestRock, int r, int s)>();

            
            int j = 7644;
            for (int r = 1; r <= (int) rem; r++)
            {
                if (false)
                {
                    bool allRock = true;
                    for (int x = 0; x < 8; x++)
                    {
                        if (!rocks.Contains((x, tallestRock)))
                        {
                            allRock = false;
                            break;
                        }
                    }

                    if (allRock)
                    {
                        //Running the input this finds these two lines
                        //All rock found at 1912, S: 1, J: 7644
                        //All rock found at 4623, S: 1, J: 7644
                        //from this we can see there is a repeating pattern that starts after 1276 rocks, and repeats every 1735 rocks for 2711 height gain
                        //the pattern starts at from s=1, j=7644
                        //so we just need to find the remainder that takes us to the 1e12 rocks max height
                        Console.WriteLine($"All rock found at {tallestRock}, S:{r % shapes.Length}, J:{j % jets.Length}, r:{r}");
                        repeats.Add((tallestRock, r % shapes.Length, j % jets.Length));
                    }
                }

                int currentX = 3;
                int currentY = tallestRock + 4;

                while (true)
                {
                    //try blow with jet
                    var nx = currentX + (jets[j % jets.Length] == '>' ? 1 : -1);
                    if (!anyHit(nx, currentY, r))
                    {
                        currentX = nx;
                    }
                    j++;

                    //now try and move down
                    var ny = currentY - 1;
                    if (!anyHit(currentX, ny, r))
                    {
                        currentY = ny;
                    }
                    else
                    {
                        goto stopped;
                    }

                }
            stopped:;
                foreach ((int ox, int oy) in shapes[r % shapes.Length])
                {
                    rocks.Add((currentX + ox, currentY + oy));
                    if (currentY + oy > tallestRock)
                    {
                        tallestRock = currentY + oy;
                    }
                }
            }

            var part1 = tallestRock;

            ulong part2 = 1912 + (ulong) tallestRock;
            part2 += div * 2711;

            Console.WriteLine($"Part1: {part1} in {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Part2: {part2} in {watch.ElapsedMilliseconds}ms");
        }

        private static bool anyHit(int x, int y, int r)
        {
            foreach ((int ox, int oy) in shapes[r % shapes.Length])
            {
                var next = (x + ox, y + oy);
                if (rocks.Contains(next))
                {
                    return true;
                }
            }

            return false;
        }

        private static string print()
        {
            var maxY = rocks.Where(tp => tp.x > 0 && tp.x < 8).Max(tp => tp.y);

            List<StringBuilder> rows = new List<StringBuilder>();
            var sb = new StringBuilder();
            sb.AppendLine(" +-------+");
            rows.Add(sb);
            for (int y = 1; y <= maxY; y++)
            {
                var sb2 = new StringBuilder();
                sb2.AppendLine("|.......|");
                rows.Add(sb2);
            }

            foreach (var rock in rocks)
            {
                if (rock.y > 0 && rock.y <= maxY)
                {
                    rows[rock.y][rock.x] = '#';
                }
            }

            return String.Join("", rows.Select(sb => sb.ToString()).Reverse());
        }
    }
}