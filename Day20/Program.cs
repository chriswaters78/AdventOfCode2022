using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Day20
{
    internal class Program
    {
        public class Node : IEnumerable<long>
        {
            public Node Previous;
            public Node Next;
            public long Value;

            public IEnumerator<long> GetEnumerator()
            {
                var current = this;
                while (true)
                {
                    yield return current.Value;
                    current = current.Next;
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            var numbers = File.ReadAllLines($"{args[0]}.txt").Select(long.Parse).ToList();
            
            Console.WriteLine($"Part 1: {solve(createLinkedList(numbers, 1), 1)} in {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            Console.WriteLine($"Part 2: {solve(createLinkedList(numbers, 811589153), 10)} in {watch.ElapsedMilliseconds}ms");
        }
        
        private static long solve(List<Node> nodes, int repetitions)
        {
            for (int r = 0; r < repetitions; r++)
            {
                foreach (var node in nodes)
                {
                    //moving L-1 places brings us back to the start
                    var absValue = Math.Abs(node.Value) % (nodes.Count - 1);
                    for (int i = 0; i < absValue; i++)
                    {
                        moveRight(node.Value > 0 ? node : node.Previous);
                    }
                    //Console.WriteLine($"Moved {node.Value} {node.Value}");
                    //Console.WriteLine(print(nodes.First()));
                }
            }

            var answers = nodes.Find(node => node.Value == 0).Take(3001).ToList();

            return answers[1000] + answers[2000] + answers[3000];
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

        private static List<Node> createLinkedList(List<long> numbers, int multiplier)
        {
            var nodes = new List<Node>();
            foreach ((var value, var index) in numbers.Select((value, i) => (value, i)))
            {
                var node = new Node() { Value = value * (long)multiplier };
                if (index > 0)
                {
                    node.Previous = nodes[index - 1];
                    nodes[index - 1].Next = node;
                }
                nodes.Add(node);
            }
            nodes.Last().Next = nodes.First();
            nodes.First().Previous = nodes.Last();

            return nodes;
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
    }
}