using System.Text;

namespace Day13
{
    internal class Program
    {
        public interface INode { };
            
        public class VNode : INode
        {
            public int Val { get; set; }
        }

        public class LNode : INode
        {
            public List<INode> Nodes { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Part1: {part1()}");
            Console.WriteLine($"Part2: {part2()}");
        }

        static int part2()
        {
            var lines = File.ReadAllLines("input.txt").Where(str => !String.IsNullOrEmpty(str))
                .Concat(new[] { "[[6]]", "[[2]]" })
                .ToList();

            //the most inefficient sort ever
            var solved = (from left in lines
                         from right in lines
                         where left != right
                         && compare(parse(left).node, parse(right).node) == 1
                         select new { left, right })
            .GroupBy(a => a.left)
            .OrderByDescending(grp => grp.Count())
            .Select(grp => grp.Key).ToList();

            return (solved.IndexOf("[[2]]") + 1) * (solved.IndexOf("[[6]]") + 1);
        }
        static int part1()
        {
            var lines = File.ReadAllText("input.txt").Split($"{Environment.NewLine}{Environment.NewLine}")
                .Select(pair => pair.Split(Environment.NewLine)).Select(arr => (arr[0], arr[1])).ToList();

            int part1 = 0;
            for (int i = 1; i <= lines.Count; i++)
            {
                var pair = lines[i - 1];
                var parse1 = parse(pair.Item1);
                var parse2 = parse(pair.Item2);

                if (compare(parse1.node, parse2.node) == 1)
                {
                    part1 += i;
                }
            }

            return part1;
        }

        public static int compare(INode node1, INode node2)
        {
            if (node1 is VNode v1 && node2 is VNode v2)
            {
                if (v1.Val < v2.Val)
                {
                    return 1;
                }
                else if (v1.Val > v2.Val)
                {
                    return -1;
                }

                return 0;
            }
            
            if (node1 is LNode l1 && node2 is LNode l2)
            {
                if (!l1.Nodes.Any() && !l2.Nodes.Any())
                {
                    return 0;
                }
                if (!l1.Nodes.Any() && l2.Nodes.Any())
                {
                    return 1;
                }
                if (l1.Nodes.Any() && !l2.Nodes.Any())
                {
                    return -1;
                }

                var result = compare(l1.Nodes[0], l2.Nodes[0]);
                if (result != 0)
                {
                    return result;
                }

                return compare(new LNode { Nodes = l1.Nodes.Skip(1).ToList() }, new LNode { Nodes = l2.Nodes.Skip(1).ToList() });
            }

            if (node1 is VNode)
            {
                node1 = new LNode() { Nodes = new List<INode>() { node1 } };
            }
            if (node2 is VNode)
            {
                node2 = new LNode() { Nodes = new List<INode>() { node2 } };
            }

            return compare(node1, node2);
        }

        public static (INode node, string remainder) parse(string input)
        {
            LNode result = new LNode() { Nodes = new List<INode>() };

            while (true)
            {
                if (!input.Any())
                {
                    return (result, "");
                }

                switch (input[0])
                {
                    case char digit when Char.IsDigit(digit):
                        var number = int.Parse(String.Join("", input.TakeWhile(ch => Char.IsDigit(ch))));

                        result.Nodes.Add(new VNode() { Val = number });
                        input = String.Join("", input.SkipWhile(ch => Char.IsDigit(ch)));
                        break;
                    case '[':
                        (var node, input) = parse(input.Substring(1));
                        result.Nodes.Add( node );
                        break;
                    case ']':
                        return (result, input.Substring(1));
                    case ',':
                        input = input.Substring(1);
                        break;
                }
            }            
        }

    }
}