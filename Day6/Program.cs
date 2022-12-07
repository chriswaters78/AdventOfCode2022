namespace Day6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var line = File.ReadAllText("input.txt");
            //find first character where previous 4 characters are all unique
            int part1 = -1;
            var queue = new Queue<char>(line.Take(14));
            for (int n = 13; n < line.Length; n++)
            {
                var c = line[n];
                if (queue.Distinct().Count() == 14)
                {
                    part1 = n;
                    break;
                }
                else
                {
                    queue.Dequeue();
                    queue.Enqueue(c);
                }
            }
        }
    }
}