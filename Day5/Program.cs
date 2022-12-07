namespace Day5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a1 = part1();
            var a2 = part2();
        }

        private static string part1()
        {
            var input = File.ReadAllLines("input.txt");
            var noStacks = (input.First().Length + 1) / 4;
            var highestStack = input.TakeWhile(str => !String.IsNullOrEmpty(str)).Count() - 1;

            var stacks = Enumerable.Range(0, noStacks).Select(_ => new Stack<char>()).ToList();


            foreach (var line in input.Take(highestStack).Reverse())
            {
                for (int n = 0; n < noStacks; n++)
                {
                    if (line[n * 4 + 1] != ' ')
                    {
                        stacks[n].Push(line[n * 4 + 1]);
                    }
                }
            }

            foreach (var instr in input.Skip(highestStack + 2))
            {
                var parts = instr.Split(' ');

                for (int x = 0; x < int.Parse(parts[1]); x++)
                {
                    stacks[int.Parse(parts[5]) - 1].Push(stacks[int.Parse(parts[3]) - 1].Pop());
                }
            }

            return String.Join("", stacks.Select(stack => stack.Peek()));
        }

        private static string part2()
        {
            var input = File.ReadAllLines("input.txt");
            var noStacks = (input.First().Length + 1) / 4;
            var highestStack = input.TakeWhile(str => !String.IsNullOrEmpty(str)).Count() - 1;

            var stacks = Enumerable.Range(0, noStacks).Select(_ => new Stack<char>()).ToList();


            foreach (var line in input.Take(highestStack).Reverse())
            {
                for (int n = 0; n < noStacks; n++)
                {
                    if (line[n * 4 + 1] != ' ')
                    {
                        stacks[n].Push(line[n * 4 + 1]);
                    }
                }
            }

            foreach (var instr in input.Skip(highestStack + 2))
            {
                var parts = instr.Split(' ');

                List<char> toMove = new List<char>();
                for (int x = 0; x < int.Parse(parts[1]); x++)
                {
                    toMove.Add(stacks[int.Parse(parts[3]) - 1].Pop());                    
                }

                toMove.Reverse();
                foreach (var c in toMove)
                {
                    stacks[int.Parse(parts[5]) - 1].Push(c);
                }
            }

            return String.Join("", stacks.Select(stack => stack.Peek()));
        }

    }
}