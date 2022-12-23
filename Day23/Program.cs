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

        static (int dr, int dc)[] cMoves = new (int dr, int dc)[] {
            (1,0),
            (-1,0),
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
            while (rounds < 10)
            {
                var moves = new List<(int r, int c)>();
                foreach ((int r, int c) in elves)
                {
                    if (considerations[rounds % 4]
                        .Select(tp => (r + tp.dr, c + tp.dc))
                        .All(tp => !elves.Contains(tp)))
                    {
                        moves.Add((r, c));
                    }
                }

                //have all the moves
                moves = moves.GroupBy(tp => tp).Where(grp => grp.Count() == 1).Select(grp => grp.Key).ToList();
                foreach (var move in moves)
                {
                    elves.Remove(move);
                    elves.Add((move.r + cMoves[rounds % 4].dr, move.c + cMoves[rounds % 4].dc));
                }
                rounds++;
            }

            (int minX, int maxX, int minY, int maxY) = (elves.Min(tp => tp.c), elves.Max(tp => tp.c), elves.Min(tp => tp.r), elves.Max(tp => tp.c));
            int part1 = (maxY - minY) * (maxX - minX) - elves.Count;
        }
    }
}
