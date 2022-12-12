namespace Day12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var maze = File.ReadAllLines("input.txt").Select(line => line.Select(ch => ch).ToArray()).ToArray();

            var SS = maze.SelectMany((line, r) => line.Select((ch, c) => (ch, r, c)).Where(t => t.ch == 'S').Select(tp => (tp.r, tp.c))).First();
            var E = maze.SelectMany((line, r) => line.Select((ch, c) => (ch, r, c)).Where(t => t.ch == 'E').Select(tp => (tp.r, tp.c))).First();

            maze[SS.r][SS.c] = 'a';
            maze[E.r][E.c] = 'z';
            
            int part2 = int.MaxValue;
            for (int r = 0; r < maze.Length; r++)
            {
                for (int c = 0; c < maze.Length; c++)
                {
                    if (maze[r][c] != 'a')
                    {
                        continue;
                    }

                    var S = (r, c);
                    
                    var nodeQueue = new Queue<(int r, int c)>();
                    nodeQueue.Enqueue(S);

                    var best = new Dictionary<(int r, int c), int>();
                    best[S] = 0;
                    while (nodeQueue.Any())
                    {
                        var curr = nodeQueue.Dequeue();
                        var heightCurr = maze[curr.r][curr.c];

                        var offsets = new (int r, int c)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
                        foreach (var offset in offsets)
                        {
                            (int r, int c) next = (curr.r + offset.r, curr.c + offset.c);
                            if (next.r >= 0 && next.r < maze.Length && next.c >= 0 && next.c < maze.First().Length)
                            {
                                if (maze[next.r][next.c] - heightCurr <= 1)
                                {
                                    if (!best.ContainsKey(next))
                                    {
                                        best[next] = best[curr] + 1;
                                        nodeQueue.Enqueue(next);
                                    }
                                }
                            }
                        }
                    }

                    if (best.ContainsKey(E))
                    {
                        part2 = Math.Min(part2, best[E]);
                    }
                }
            }

        }
    }
}