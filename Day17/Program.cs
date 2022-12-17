using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System;
using System.Runtime.Intrinsics.Arm;

namespace Day17
{
    internal class Program
    {
        static (int ox, int oy)[][] shapes = new[] {
                new [] {(0,0), (1,0), (2,0), (3,0) },
                new [] {(0,1), (1,0), (1,1), (1,2), (2,1) },
                new [] {(0,0), (1,0), (2,0), (2,1), (2,2)},
                new [] {(0,0), (0,1), (0,2), (0,3)},
                new [] {(0,0), (0,1), (1,0), (1,1)},
            };

        static char[] jets;        

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            jets = File.ReadAllText($"{args[0]}.txt").Select(ch => ch).ToArray();

            var part1 = simulate(0, 0, 2022);
            Console.WriteLine($"Part1: {part1} in {watch.ElapsedMilliseconds}ms");

            watch.Restart();

            //find the leftover from when the pattern repeates
            ulong ROCKS = 1000000000000;
            ROCKS -= 1276;
            ulong div = ROCKS / 1735;
            ulong rem = ROCKS % 1735;

            //then answer = 1912 + div * 2711 + tallestRock
            ulong part2 = 1912 + (ulong)simulate(1, 7644, (int) rem);
            part2 += div * 2711;

            Console.WriteLine($"Part2: {part2} in {watch.ElapsedMilliseconds}ms");
        }

        private static int simulate(int rs, int js, int nr)
        {
            int j = js;
            int tallestRock = 0;
            var rocks = new HashSet<(int x, int y)>();
            for (int x = 0; x < 9; x++)
            {
                rocks.Add((x, 0));
            }
            for (int y = 0; y < 2 * nr; y++)
            {
                rocks.Add((0, y));
                rocks.Add((8, y));
            }

            for (int r = rs; r <= rs + nr - 1; r++)
            {
                if (Enumerable.Range(1, 7).All(x => rocks.Contains((x, tallestRock))))
                {
                    //Running the input for longer than 2022 rocks finds this as the first all rock top layer repeate
                    //All rock found at 0, S: 0, J: 0, r: 0
                    //All rock found at 1912, S: 1, J: 7644, r: 1276
                    // ...
                    //All rock found at 4623, S: 1, J: 7644, r: 3011
                    //This starts after 1276 rocks, and repeats every 1735 rocks for 2711 height gain each time
                    //The pattern starts at from s=1, j=7644
                    //For part 2 we then just need to find the remainder that takes us to the 1e12 rocks max height
                    Console.WriteLine($"All rock found at {tallestRock}, S:{r % shapes.Length}, J:{j % jets.Length}, r:{r}");
                }

                int currentX = 3;
                int currentY = tallestRock + 4;

                while (true)
                {
                    //try blow with jet
                    var nx = currentX + (jets[j % jets.Length] == '>' ? 1 : -1);
                    if (!anyHit(rocks, nx, currentY, r))
                    {
                        currentX = nx;
                    }
                    j++;

                    //now try and move down
                    var ny = currentY - 1;
                    if (!anyHit(rocks, currentX, ny, r))
                    {
                        currentY = ny;
                    }
                    else
                    {
                        break;
                    }
                }
                
                //add shape to rock set in it's final resting place
                //and update the tallest rock if this shape is higher than the current
                foreach (var offset in shapes[r % shapes.Length])
                {
                    rocks.Add((currentX + offset.ox, currentY + offset.oy));
                    if (currentY + offset.oy > tallestRock)
                    {
                        tallestRock = currentY + offset.oy;
                    }
                }
            }

            return tallestRock;
        }

        private static bool anyHit(HashSet<(int x, int y)> rocks, int x, int y, int r)
        {
            return shapes[r % shapes.Length].Any(offset => rocks.Contains((x + offset.ox, y + offset.oy)));
        }

        private static string print(HashSet<(int x, int y)> rocks)
        {
            var maxY = rocks.Where(tp => tp.x > 0 && tp.x < 8).Max(tp => tp.y);

            List<StringBuilder> rows = new List<StringBuilder>();
            var sb = new StringBuilder();
            sb.AppendLine(" +-------+");
            rows.Add(sb);
            for (int y = 1; y <= maxY; y++)
            {
                var sb2 = new StringBuilder();
                sb2.AppendLine("#.......#");
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