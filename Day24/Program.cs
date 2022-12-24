using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Day24
{
    internal class Program
    {
        record struct State((int r, int c) position, int t);

        static HashSet<(int r, int c)>[] grids;
        static int R;
        static int C;
        static int LCM;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            grids = generateBlizzardGrids(args[0]);

            var part1 = solve(new State((-1, 0), 0), (R, C - 1));
            var endToStart = solve(new State((R, C - 1), part1), (-1, 0));
            var part2 = solve(new State((-1, 0), endToStart), (R, C-1));

            Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Part 2: {part2}");
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}ms");
        }

        private static int solve(State initialState, (int r, int c) goal)
        {
            var cache = new Dictionary<State, int>();
            var queue = new Queue<State>();
            queue.Enqueue(initialState);

            while (queue.Any())
            {
                var state = queue.Dequeue();
                if (!cache.TryAdd(state with { t = state.t % LCM }, state.t))
                {
                    continue;
                }
                if (state.position.r == goal.r && state.position.c == goal.c)
                {
                    return state.t;
                }

                foreach ((int dr, int dc) offset in new[] { (1, 0), (0, 1), (0, 0), (-1, 0), (0, -1) })
                {
                    var newState = state with { t = state.t + 1, position = (state.position.r + offset.dr, state.position.c + offset.dc) };
                    if (!grids[(state.t + 1) % LCM].Contains(newState.position))
                    {
                        queue.Enqueue(newState);
                    }
                }
            }

            throw new Exception($"Unsolvable!");
        }

        private static HashSet<(int r, int c)>[] generateBlizzardGrids(string input)
        {
            var lines = File.ReadAllLines($"{input}.txt");
            R = lines.Length - 2;
            C = lines.First().Length - 2;
            LCM = R * C / (int)BigInteger.GreatestCommonDivisor(R, C);

            var grids = new HashSet<(int r, int c)>[LCM];

            for (int t = 0; t < LCM; t++)
            {
                grids[t] = new HashSet<(int r, int c)>();
                for (int r = 0; r < R; r++)
                {
                    for (int c = 0; c < C; c++)
                    {
                        switch (lines[r + 1][c + 1])
                        {
                            case '<':
                                grids[t].Add((r, (c + LCM - t) % C));
                                break;
                            case '>':
                                grids[t].Add((r, (c + t) % C));
                                break;
                            case 'v':
                                grids[t].Add(((r + t) % R, c));
                                break;
                            case '^':
                                grids[t].Add(((r + LCM - t) % R, c));
                                break;
                        }
                    }
                }

                for (int r = -2; r < R + 2; r++)
                {
                    grids[t].Add((r, -1));
                    grids[t].Add((r, C));
                }
                for (int c = -1; c < C + 1; c++)
                {
                    grids[t].Add((-1, c));
                    grids[t].Add((-2, c));
                    grids[t].Add((R, c));
                    grids[t].Add((R + 1, c));
                }
                grids[t].Remove((-1, 0));
                grids[t].Remove((R, C - 1));
            }

            return grids;
        }

        static string print(HashSet<(int r, int c)> grid, State state)
        {
            List<StringBuilder> sbs = new List<StringBuilder>();
            for (int r = -2; r <= R + 1; r++)
            {
                var thisRow = new StringBuilder();
                sbs.Add(thisRow);
                for (int c = -1; c <= C; c++)
                {
                    if (state.position.r == r && state.position.c == c)
                    {
                        thisRow.Append('E');
                    }
                    else
                    {
                        thisRow.Append(grid.Contains((r, c)) ? '#' : '.');
                    }
                }
            }

            return String.Join(Environment.NewLine, sbs.Select(sb => sb.ToString()));
        }
    }
}