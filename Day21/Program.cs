using System.IO;
using System.Numerics;
using static Day21.Program;

namespace Day21
{
    internal class Program
    {
        public class Node
        {
            public string Name;
        }

        public class CNode : Node
        {            
            public char Operation;
            public string Node1;
            public string Node2;
        }
        public class VNode : Node
        {
            public BigInteger Value;
        }
        static Dictionary<string, Node> nodes;
        static void Main(string[] args)
        {
            nodes = new Dictionary<string, Node>();
            foreach (var arr in File.ReadAllLines($"{args[0]}.txt").Select(str => str.Split(": ")))
            {
                if (BigInteger.TryParse(arr[1], out BigInteger value))
                {
                    nodes.Add(arr[0], new VNode() { Name = arr[0], Value = value });
                }
                else
                {
                    var op = arr[1].Split(' ');
                    nodes.Add(arr[0], new CNode() { Name = arr[0], Operation = op[1][0], Node1 = op[0], Node2 = op[2] });
                }
            }

            var values1 = solve(true);
            var part1 = values1["root"].nominator / values1["root"].denominator;
            Console.WriteLine($"Part 1: {part1}");

            var values2 = solve(false);
            Console.WriteLine($"Part 2, solve this equation (https://www.mathpapa.com/equation-solver/)");
            Console.WriteLine(getEquation(values2, "root"));

            var part2 = 801541236563053696 / 232925;
            Console.WriteLine($"Part 2: {part2}");
        }

        private static Dictionary<string, (BigInteger nominator, BigInteger denominator)> solve(bool part1)
        {
            var values = new Dictionary<string, (BigInteger nominator, BigInteger denominator)>();
            bool noHits;
            do
            {
                noHits = true;
                foreach (var node in nodes.Values.Where(node => !values.ContainsKey(node.Name) && (part1 || node.Name != "humn")))
                {
                    if (node is VNode)
                    {
                        values.Add(node.Name, (((VNode)node).Value, 1));
                        noHits = false;
                    }
                    else
                    {
                        var cnode = node as CNode;
                        if (values.ContainsKey(cnode.Node1) && values.ContainsKey(cnode.Node2))
                        {
                            noHits = false;
                            BigInteger nominator;
                            BigInteger denominator;
                            switch (cnode.Operation)
                            {
                                case '+':
                                    nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator + values[cnode.Node2].nominator * values[cnode.Node1].denominator;
                                    denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                    break;
                                case '-':
                                    nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator - values[cnode.Node2].nominator * values[cnode.Node1].denominator;
                                    denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                    break;
                                case '*':
                                    nominator = values[cnode.Node1].nominator * values[cnode.Node2].nominator;
                                    denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                    break;
                                default:
                                    nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator;
                                    denominator = values[cnode.Node1].denominator * values[cnode.Node2].nominator;
                                    break;
                            }
                            var gcd = BigInteger.GreatestCommonDivisor(nominator, denominator);
                            values.Add(cnode.Name, (nominator / gcd, denominator / gcd));
                        }
                    }
                }
            } while (!noHits);

            return values;
        }

        private static string getEquation(Dictionary<string, (BigInteger nominator, BigInteger denominator)> values, string nodeName)
        {
            if (values.ContainsKey(nodeName))
            {
                if (values[nodeName].denominator != 1)
                {
                    throw new Exception($"Denominator not 1!");
                }
                return values[nodeName].nominator.ToString();
            }
            else
            {
                if (nodeName == "humn")
                {
                    return "h";
                }
                var node = nodes[nodeName] as CNode;
                return $"({getEquation(values, node.Node1)}) {(nodeName == "root" ? "=" : node.Operation)} ({getEquation(values, node.Node2)})";
            }
        }
    }
}