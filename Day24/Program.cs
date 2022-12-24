using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Day24
{
    internal class Program
    {
        record struct State(int r, int c, int t, int p);

        static HashSet<(int r, int c)>[] grids;
        static Dictionary<State, int> cache = new Dictionary<State, int>();
        static int R;
        static int C;
        static int LCM;

        static void Main(string[] args)
        {
            //input is 120 x 25
            //2 x 2 x 2 x 3 x 5
            // 5 x 5
            //LCM = 600

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var lines = File.ReadAllLines($"{args[0]}.txt");
            R = lines.Length - 2;
            C = lines.First().Length - 2;
            LCM = R * C / (int)BigInteger.GreatestCommonDivisor(R, C);

            grids = new HashSet<(int r, int c)>[LCM];

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
                                //move LCM - i places to right
                                grids[t].Add((r, (c + LCM - t) % C));
                                break;
                            case '>':
                                //move t places to right
                                grids[t].Add((r, (c + t) % C));
                                break;
                            case 'v':
                                //move t places down
                                grids[t].Add(((r + t) % R, c));
                                break;
                            case '^':
                                //move LCM - t t places up
                                grids[t].Add(((r + LCM - t) % R, c));
                                break;
                        }
                    }
                }

                for (int r = -2; r < R+2; r++)
                {
                    grids[t].Add((r, -1));
                    grids[t].Add((r, C));
                }
                for (int c = -1; c < C+1; c++)
                {
                    grids[t].Add((-1, c));
                    grids[t].Add((-2, c));
                    grids[t].Add((R, c));
                    grids[t].Add((R + 1, c));
                }
                grids[t].Remove((-1, 0));
                grids[t].Remove((R, C - 1));
            }

            //Console.WriteLine(print(grids[0], new State(-1, 0, 0, 0)));
            //Console.WriteLine();
            //Console.WriteLine(print(grids[1], new State(-1, 0, 0, 0)));

            solve(new State(-1, 0, 0, 0));
            int best1 = int.MaxValue;
            int best2 = int.MaxValue;
            for (int t = 0; t < LCM; t++)
            {
                var finalState1 = new State(R, C - 1, t, 1);
                var finalState2 = new State(R, C - 1, t, 2);
                if (cache.ContainsKey(finalState1))
                {
                    best1 = Math.Min(best1, cache[finalState1]);
                }
                if (cache.ContainsKey(finalState2))
                {
                    best2 = Math.Min(best2, cache[finalState2]);
                }
            }

            Console.WriteLine($"Part 1: {best1}");
            Console.WriteLine($"Part 2: {best2}");
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}ms");
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
                    if (state.r == r && state.c == c)
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

        //for every possible starting t
        //do a DFS tracking best state to given position
        private static void solve(State initialState)
        {
            Queue<State> queue = new Queue<State>();
            queue.Enqueue(initialState);

            while (queue.Any())
            {
                var state = queue.Dequeue();

                //Console.WriteLine($"T={state.t}");
                //Console.WriteLine(print(grids[state.t % LCM], state));

                if (state.r == R && state.c == C - 1)
                {
                    if (state.p == 0)
                    {
                        state = state with { p = 1 };
                    }
                }
                if (state.r == -1 && state.c == 0)
                {
                    if (state.p == 1)
                    {
                        state = state with { p = 2 };
                    }
                }
                var cacheKey = state with { t = state.t % LCM };
                if (cache.TryGetValue(cacheKey, out int result))
                {
                    if (state.t >= result)
                    {
                        continue;
                    }
                }

                cache[cacheKey] = state.t;
                if (state.r == R && state.c == C - 1 && state.p == 2)
                {
                    continue;
                }

                var nextGrid = grids[(state.t + 1) % LCM];
                //down
                if (!nextGrid.Contains((state.r + 1, state.c)))
                {
                    queue.Enqueue(state with { t = state.t + 1, r = state.r + 1 });
                }
                //right
                if (!nextGrid.Contains((state.r, state.c + 1)))
                {
                    queue.Enqueue(state with { t = state.t + 1, c = state.c + 1 });
                }
                //wait
                if (!nextGrid.Contains((state.r, state.c)))
                {
                    queue.Enqueue(state with { t = state.t + 1 });
                }
                //up
                if (!nextGrid.Contains((state.r - 1, state.c)))
                {
                    queue.Enqueue(state with { t = state.t + 1, r = state.r - 1 });
                }
                //left
                if (!nextGrid.Contains((state.r, state.c - 1)))
                {
                    queue.Enqueue(state with { t = state.t + 1, c = state.c - 1 });
                }
            }
        }
    }
}