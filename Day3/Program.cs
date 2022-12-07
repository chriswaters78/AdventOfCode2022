namespace Day3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a1 = part1();
            var a2 = part2();
        }

        private static int part1()
        {
            var lines = File.ReadAllLines("input.txt");
            var total = 0;
            foreach (var line in lines)
            {
                var common = line.Take(line.Count() / 2).Intersect(line.Skip(line.Count() / 2));
                total += getPriority(common.Single());
            }

            return total;
        }

        private static int part2()
        {
            var lines = File.ReadAllLines("input.txt");
            var total = 0;
            foreach (var line in lines.Select((line, i) => (line, i)).GroupBy(tp => tp.i / 3).Select(grp => grp.Select(tp => tp.line).ToList()))
            {
                var common = line[0].Intersect(line[1]).Intersect(line[2]);
                total += getPriority(common.Single());
            }

            return total;
        }

        private static int getPriority(char c)
        {
            return c >= 'a' ? c - 'a' + 1 :  c - 'A' + 27;
        }
    }
}