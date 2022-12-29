using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Linq;

namespace Day16
{
    internal class Program
    {
        static Dictionary<int, (int, List<int>)> indexGraph;
        static int[,] allPairs;

        const int MINUTES = 26;
        const int MAX = 10000;

        static void Main(string[] args)
        {
            var stringGraph = File.ReadAllLines($"{args[0]}.txt")
                .Select(
                        str => (str[6..8],
                        int.Parse(str[23..25].Trim(';')),
                        (str.Contains("valves") ? str.Split("valves ")[1] : str.Split("valve ")[1]).Split(", ").ToArray()
                    )).ToDictionary(tp => tp.Item1, tp => (tp.Item2, tp.Item3));

            var nonZeroCount = stringGraph.Where(kvp => kvp.Value.Item1 != 0).Count();

            var stringToIndex = stringGraph.OrderByDescending(kvp => kvp.Value.Item1).Select((kvp, i) => (kvp.Key, i)).ToDictionary(tp => tp.Key, tp => tp.i);
            var indexToString = stringToIndex.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            indexGraph = stringGraph.ToDictionary(kvp => stringToIndex[kvp.Key], kvp => (kvp.Value.Item1, kvp.Value.Item2.Select(str => stringToIndex[str]).ToList()));

            //partition our valve sets into disjoint sets
            var sets = new List<(List<int>, List<int>)>();
            for (ulong i = 0; i < Math.Pow(2, nonZeroCount); i++)
            {
                var list1 = new List<int>();
                var list2 = new List<int>();
                for (int b = 0; b < nonZeroCount; b++)
                {
                    if (IsBitSet(b, i))
                    {
                        list1.Add(b);
                    }
                    else
                    {
                        list2.Add(b);
                    }
                }
                sets.Add((list1, list2));
            }
            Console.WriteLine($"{sets.Count} disjoint sets found");

            //get all pairs shortest paths using Floyd-Warshall
            //note this is the DISTANCE only
            //then for each disjoint item, recursively each side of the set, starting at AA
            //and find the maximum pressure relieved within the time limit
            //the maximum of all of these is the answer
            allPairs = FloydWarshall(indexGraph);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int best = 0;
            foreach (var pair in sets)
            {
                Console.WriteLine($"Testing set: [{String.Join(", ", pair.Item1)}] and [{String.Join(", ", pair.Item2)}]");
                //solve for each side and add answers
                var set1Best = solve(pair.Item1, 0, 0, 0, stringToIndex["AA"]);
                Console.WriteLine($"Set 1: {set1Best}");
                var set2Best = solve(pair.Item2, 0, 0, 0, stringToIndex["AA"]);
                Console.WriteLine($"Set 2: {set2Best}");
                best = Math.Max(best, set1Best + set2Best);
            }

            Console.WriteLine($"Part2: {best} in {watch.ElapsedMilliseconds}ms");
        }

        static bool IsBitSet(int bit, ulong state)
        {
            return (state & (1UL << bit)) != 0;
        }
        static ulong SetBit(int bit, ulong state)
        {
            return state |= 1UL << bit;
        }

        //static int maxFlowLeft(ulong state)
        //{
        //    int flow = 0;
        //    for (int i = 0; i < graph2.Count; i++)
        //    {
        //        if (!IsBitSet(i, state))
        //        {
        //            flow += graph2[i].flow;
        //        }
        //    }

        //    return flow;
        //}

        public static int[,] FloydWarshall(Dictionary<int, (int, List<int>)> graph)
        {
            int[,] distance = new int[graph.Count, graph.Count];
            for (int i = 0; i < graph.Count; i++)
            {
                for (int j = 0; j < graph.Count; j++)
                {
                    distance[i, j] = MAX;
                }
            }

            foreach (var key in graph.Keys)
            {
                foreach (var edge in graph[key].Item2)
                {
                    distance[key, edge] = 1;
                }
            }

            for (int k = 0; k < graph.Count; ++k)
            {
                for (int i = 0; i < graph.Count; ++i)
                {
                    for (int j = 0; j < graph.Count; ++j)
                    {
                        if (distance[i, k] + distance[k, j] < distance[i, j])
                            distance[i, j] = distance[i, k] + distance[k, j];
                    }
                }
            }

            return distance;
        }

        static int solve(List<int> toOpen, int timespent, int currentflow, ulong valves, int currentValve)
        {
            //we are trying to open all valves in toOpen
            //we have opened (and added the final score) for all valves with bits set in valves
            //we have to try each valve remaining in toOpen

            int best = currentflow;
            foreach (var nextValve in toOpen)
            {
                if (!IsBitSet(nextValve, valves))
                {
                    //try opening this one next
                    var timeToValve = allPairs[currentValve, nextValve];
                    if (timeToValve + timespent < MINUTES)
                    {
                        //we can reach AND open it
                        int nextFlow = currentflow + (MINUTES - (timeToValve + timespent + 1)) * indexGraph[nextValve].Item1;
                        var nextValves = SetBit(nextValve, valves);
                        int nextBest = solve(toOpen, timespent + timeToValve, nextFlow, nextValves, nextValve);
                        best = Math.Max(best, nextBest);
                    }
                }
            }

            return best;

            //if (maxFlowLeft(valves) * (MINUTES - timespent) + currentflow < best)
            //{
            //    //not possible to improve from here
            //    return;
            //}

        }
    }
}