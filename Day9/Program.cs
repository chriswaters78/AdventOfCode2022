using System.Text;

namespace Day9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var moves = File.ReadLines("input.txt").Select(str =>str.Split(' ')).Select(arr => (arr[0][0], int.Parse(arr[1])));

            const int KNOTS = 10;
            var knots = Enumerable.Repeat<(int r, int c)>((0, 0), KNOTS).ToArray();
            
            var visited = new HashSet<(int r, int c)>(new[] { knots[0] });

            int steps = 0;

            print(steps, knots);
            foreach (var move in moves)
            {
                var head = knots[KNOTS-1];
                switch (move.Item1)
                {
                    case 'L':
                        head.c -= move.Item2;
                        break;
                    case 'R':
                        head.c += move.Item2;
                        break;
                    case 'D':
                        head.r -= move.Item2;
                        break;
                    case 'U':
                        head.r += move.Item2;
                        break;                                           
                }

                knots[KNOTS-1] = head;

                steps++;

                while (true)
                {
                    bool anyMoved = false;
                    for (int k = KNOTS - 2; k >= 0; k--)
                    {
                        head = knots[k + 1];
                        var tail = knots[k];
                        var moveResult = moveTail(tail, head);
                        anyMoved |= moveResult.Item2;
                        tail = moveResult.Item1;
                        knots[k] = tail;

                        if (k == 0)
                        {
                            visited.Add(tail);
                        }
                    }
                    steps++;
                    //print(steps, knots);

                    if (!anyMoved)
                    {
                        break;
                    }
                }

            }

            Console.WriteLine($"Part 1: {visited.Count}");
            //print(-1, visited.ToArray());
        }

        private static ((int r, int c) tail, bool moved) moveTail((int r, int c) tail, (int r, int c) head)
        {
            var ABSr = Math.Abs(head.r - tail.r);
            var ABSc = Math.Abs(head.c - tail.c);
            bool moved = false;

            //touching
            if (ABSr <= 1 && ABSc <= 1)
            {
                return (tail, false);
            }

            var notInRowAndColumn = head.r != tail.r && head.c != tail.c;
            if (notInRowAndColumn | ABSr > 1)
            {
                tail.r += (head.r - tail.r) / ABSr;
                moved |= true;
            }
            if (notInRowAndColumn | ABSc > 1)
            {
                tail.c += (head.c - tail.c) / ABSc;
                moved |= true;
            }
            
            return (tail, moved);
        }

        public static void print(int steps, (int r, int c)[] knots)
        {
            Console.WriteLine($"Steps: {steps}");

            const int SIZE = 40;
            var builders = new StringBuilder[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                builders[i] = new StringBuilder("".PadLeft(SIZE, '.'));
            }

            foreach (var knot in knots)
            {
                builders[knot.r + (SIZE / 2)][knot.c + (SIZE / 2)] = 'X';
            }
            
            Console.WriteLine(string.Join(Environment.NewLine, builders.Reverse().Select(builder => builder.ToString())));
        }
    }
}