namespace Day23
{
    internal class Program
    {
        static (int dr, int dc)[][] considerations = new (int dr, int dc)[][] {
            new [] { (-1, -1), (-1,0), (-1,1) },
            new [] { (1, -1), (1,0), (1,1) },
            new [] { (-1, -1), (0,-1), (1,-1) },
            new [] { (-1, 1), (0,1), (1,1) },
        };

        static (int dr, int dc)[] allOffSets = new (int dr, int dc)[] {
            (-1,-1),
            (-1,0),
            (-1,1),
            (0,-1),
            (0,1),
            (1,-1),
            (1,0),
            (1,1),
        };

        static (int dr, int dc)[] cMoves = new (int dr, int dc)[] {
            (-1,0),
            (1,0),
            (0,-1),
            (0,1)
        };

        static void Main(string[] args)
        {
            var elves = new HashSet<(int r, int c)>();
            var input = File.ReadAllLines($"{args[0]}.txt");
            for (int r = 0; r < input.Length; r++)
            {
                for (int c = 0; c < input.First().Length; c++)
                {
                    if (input[r][c] == '#')
                    {
                        elves.Add((r, c));
                    }
                }
            }

            int rounds = 0;
            while (true)
            {
                var moves = new Dictionary<(int r2, int c2), List<(int r1, int c1)>>();
                foreach ((int r, int c) in elves)
                {
                    if (!allOffSets.Select(tp => (r + tp.dr, c + tp.dc)).Any(tp => elves.Contains(tp)))
                    {
                        continue;
                    }
                    for (int co = 0; co < 4; co++)
                    {
                        var check = considerations[(rounds + co) % 4].Select(tp => (r + tp.dr, c + tp.dc)).ToList();
                        if (check.All(tp => !elves.Contains(tp)))
                        {
                            var moveTo = (r + cMoves[(rounds + co) % 4].dr, c + cMoves[(rounds + co) % 4].dc);
                            if (!moves.ContainsKey(moveTo))
                            {
                                moves[moveTo] = new List<(int r1, int c1)>();
                            }
                            moves[moveTo].Add((r, c));
                            break;
                        }
                    }
                }

                //have all the moves
                bool anyMoved = false;
                foreach (var move in moves.Where(kvp => kvp.Value.Count == 1))
                {
                    anyMoved = true;
                    elves.Remove(move.Value.First());
                    elves.Add(move.Key);
                }
                rounds++;
                if (!anyMoved)
                {
                    break;
                }
            }

            //(int minX, int maxX, int minY, int maxY) = (elves.Min(tp => tp.c), elves.Max(tp => tp.c), elves.Min(tp => tp.r), elves.Max(tp => tp.r));
            //int part1 = (1 + maxY - minY) * (1 + maxX - minX) - elves.Count;
            //Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Part 2: {rounds}");
        }
    }
}
