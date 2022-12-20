using System.Text;

namespace Day20
{
    internal class Program
    {
        public class Node
        {
            public Node Previous;
            public Node Next;
            public long Value;
        }
        
        static void Main(string[] args)
        {
            List<Node> nodes = new List<Node>();
            foreach ((var value, var index) in File.ReadAllLines($"{args[0]}.txt").Select((str, i) => (long.Parse(str), i)))
            {
                var node = new Node() { Value = value * (long) 811589153 };
                if (index > 0)
                {
                    node.Previous = nodes[index - 1];
                    nodes[index - 1].Next = node;                    
                }
                nodes.Add(node);
            }
            nodes.Last().Next = nodes.First();
            nodes.First().Previous = nodes.Last();

            for (int r = 0; r < 10; r++)
            foreach (var node in nodes)
            {
                //moving L-1 places brings us back to the start
                var absValue = Math.Abs(node.Value) % (nodes.Count - 1);
                for (int i = 0; i < absValue; i++)
                {
                    if (node.Value > 0)
                    {
                        moveRight(node);
                    }
                    else
                    {
                        moveRight(node.Previous);
                    }
                }
                //Console.WriteLine($"Moved {node.Value} {node.Value}");
                //Console.WriteLine(print(nodes.First()));
             }

            var index0 = nodes.Find(node => node.Value == 0);

            var current = index0;
            var answers = new List<long>();
            for (int i = 0; i < 3000; i++)
            {
                current = current.Next;
                answers.Add(current.Value);
            }

            var part1 = answers[999] + answers[1999] + answers[2999];

            Console.WriteLine($"Part 1: {part1}");
        }

        private static string print(Node node)
        {
            List<long> ints = new List<long>();
            var current = node;
            do
            {
                ints.Add(current.Value);
                current = current.Next;
            } while (current != node);
             
            return String.Join(", ", ints);
        }
        private static void moveRight(Node node)
        {
            var node1 = node.Previous;
            var node2 = node;
            var node3 = node.Next;
            var node4 = node.Next.Next;

            node1.Next = node3;
            node3.Previous = node1;
            node3.Next = node2;
            node2.Previous = node3;
            node2.Next = node4;
            node4.Previous = node2;
        }
    }
}