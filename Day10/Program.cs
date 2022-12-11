using System.Text;

namespace Day10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var instructions = File.ReadAllLines("input.txt");

            var values = new List<(int cycle, int x)>();
            (int cycle, int X) = (1, 1);
            values.Add((1, 1));

            foreach (var instruction in instructions)
            {
                if (instruction == "noop")
                {
                    cycle++;
                    values.Add((cycle, X));
                    continue;
                }

                var value = int.Parse(instruction.Substring(5));
                cycle += 2;
                values.Add((cycle - 1, X));
                X += value;
                values.Add((cycle, X));
            }

            var part1 = 0;
            for (int i = 20; i <= 220; i+=40)
            {
                part1 += i * values[i - 1].x;
            }
            
            Console.WriteLine($"Part 1: {part1}");

            StringBuilder sb = new StringBuilder();
            for (int l = 0; l < 6; l++)
            {
                for (int c = 0; c < 40; c++)
                {
                    sb.Append(Math.Abs(values[l * 40 + c].x - c) <= 1 ? '#' : '.');                    
                }
                sb.AppendLine();
            }

            Console.WriteLine($"Part 2:");
            Console.WriteLine(sb);
        }
    }
}