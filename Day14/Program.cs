namespace Day14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var walls = File.ReadAllLines("input.txt").Select(str => str.Split(" -> ").Select(str => str.Split(",")).Select(arr => (x: int.Parse(arr[0]), y: int.Parse(arr[1]))).ToList()).ToList();

            var maxY = walls.SelectMany(wall => wall.Select(tp => tp.y)).Max();
            walls.Add(new List<(int x, int y)>() { (0, maxY + 2), (1000, maxY + 2) });

            var grid = new Dictionary<(int x, int y), char>();
            foreach (var wall in walls)
            {
                for (int i = 0; i < wall.Count - 1; i++)
                {
                    if (wall[i].x == wall[i + 1].x)
                    {
                        (var y1, var y2) = (wall[i].y, wall[i + 1].y);
                        if (y1 > y2)
                        {
                            (y1, y2) = (y2, y1);
                        }
                        for (int y = y1; y <= y2; y++)
                        {
                            grid[(wall[i].x, y)] = '#';
                        }
                    } else
                    {
                        (var x1, var x2) = (wall[i].x, wall[i + 1].x);
                        if (x1 > x2)
                        {
                            (x1, x2) = (x2, x1);
                        }
                        for (int x = x1; x <= x2; x++)
                        {
                            grid[(x, wall[i].y)] = '#';
                        }
                    }
                }
            }

            //var voidStart = grid.Max(tp => tp.Key.y);

            while (true)
            {
            nextsand:;
                (var sx, var sy) = (500, 0);
                while (true)
                {
                moveNext:;
                    foreach (var next in new[] { (sx, sy + 1), (sx - 1, sy + 1), (sx + 1, sy + 1) })
                    {
                        if (!grid.ContainsKey(next) || (grid[next] != '#' && grid[next] != 'o'))
                        {
                            (sx, sy) = next;
                            //if (sy >= voidStart)
                            //{
                            //    goto done;
                            //}
                            goto moveNext;
                        }
                    }

                    //can't move
                    if ((sx, sy) == (500, 0))
                    {
                        goto done;
                    }
                    grid[(sx, sy)] = 'o';
                    goto nextsand;
                }
            }

        done:;

            var part2 = grid.Count(kvp => kvp.Value == 'o') + 1;
        }
    }
}