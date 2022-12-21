using System.Numerics;

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

        static void Main(string[] args)
        {
            List<Node> nodes = new List<Node>();
            foreach (var arr in File.ReadAllLines($"{args[0]}.txt").Select(str => str.Split(": ")))
            {
                if (BigInteger.TryParse(arr[1], out BigInteger value))
                {
                    nodes.Add(new VNode() { Name = arr[0], Value = value });
                }
                else
                {
                    var op = arr[1].Split(' ');
                    nodes.Add(new CNode() { Name = arr[0], Operation = op[1][0], Node1 = op[0], Node2 = op[2] });
                }
            }


            var values = new Dictionary<string, (BigInteger nominator, BigInteger denominator)>();
            while (values.Count < nodes.Count)
            {
                foreach (var node in nodes.Where(node => !values.ContainsKey(node.Name)))
                {
                    if (node is VNode)
                    {
                        values.Add(node.Name, (((VNode)node).Value, 1));
                    }
                    else
                    {
                        var cnode = node as CNode;
                        if (values.ContainsKey(cnode.Node1) && values.ContainsKey(cnode.Node2))
                        {
                            if (cnode.Name == "root")
                            {
                            }
                            switch (cnode.Operation)
                            {
                                case '+':
                                    {
                                        var nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator + values[cnode.Node2].nominator * values[cnode.Node1].denominator;
                                        var denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                        values.Add(cnode.Name, (nominator, denominator));
                                    }
                                    break;
                                case '-':
                                    {
                                        var nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator - values[cnode.Node2].nominator * values[cnode.Node1].denominator;
                                        var denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                        values.Add(cnode.Name, (nominator, denominator));
                                    }
                                    break;
                                case '*':
                                    {
                                        var nominator = values[cnode.Node1].nominator * values[cnode.Node2].nominator;
                                        var denominator = values[cnode.Node1].denominator * values[cnode.Node2].denominator;
                                        values.Add(cnode.Name, (nominator, denominator));
                                    }
                                    break;
                                case '/':
                                    {
                                        var nominator = values[cnode.Node1].nominator * values[cnode.Node2].denominator;
                                        var denominator = values[cnode.Node1].denominator * values[cnode.Node2].nominator;
                                        values.Add(cnode.Name, (nominator, denominator));
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            var part1 = values["root"].nominator / values["root"].denominator;
        }
    }
}