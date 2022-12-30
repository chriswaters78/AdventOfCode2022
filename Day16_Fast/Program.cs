using System.Diagnostics;

namespace Day16
{
    internal class Program
    {
        const int MINUTES = 26;
        const int MAX = 10000;
        
        static Dictionary<int, (int flow, List<int> edges)> indexGraph;
        static int[,] allPairs;

        record struct State(int time, ulong valves, int currentValve);

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //parse graph and assign indexes to each valve
            //making sure all the valves that can be opened are assigned an index first
            var stringGraph = File.ReadAllLines($"{args[0]}.txt")
                .Select(
                        str => (str[6..8],
                        int.Parse(str[23..25].Trim(';')),
                        (str.Contains("valves") ? str.Split("valves ")[1] : str.Split("valve ")[1]).Split(", ").ToArray()
                    )).ToDictionary(tp => tp.Item1, tp => (tp.Item2, tp.Item3));

            var nonZeroCount = stringGraph.Where(kvp => kvp.Value.Item1 != 0).Count();
            var stringToIndex = stringGraph.OrderByDescending(kvp => kvp.Value.Item1).Select((kvp, i) => (kvp.Key, i)).ToDictionary(tp => tp.Key, tp => tp.i);
            indexGraph = stringGraph.ToDictionary(kvp => stringToIndex[kvp.Key], kvp => (kvp.Value.Item1, kvp.Value.Item2.Select(str => stringToIndex[str]).ToList()));

            int bitMask = (int) Math.Pow(2, nonZeroCount) - 1;
            //partition our valve sets into disjoint sets
            var sets = new List<(List<int> set1, List<int> set2)>();
            for (ulong i = 0; i < Math.Pow(2, nonZeroCount - 1); i++)
            {
                (var list1, var list2) = (new List<int>(), new List<int>());
                for (int b = 0; b < nonZeroCount; b++)
                {
                    if (IsBitSet(b, i))
                        list1.Add(b);
                    else
                        list2.Add(b);
                }
                sets.Add((list1, list2));
            }

            //get all pairs shortest paths using Floyd-Warshall
            //note this is the DISTANCE only
            allPairs = floydWarshall(indexGraph);

            //then for each disjoint item, recursively DFS each side of the set, starting at AA
            //and find the maximum pressure relieved within the time limit
            //the maximum of all of these is the answer
            var maximums = sets.Select(pair => (solve(new Dictionary<State, int>(), pair.set1, new State(0,0, stringToIndex["AA"]), 0), solve(new Dictionary<State, int>(), pair.set2, new State(0, 0, stringToIndex["AA"]), 0))).ToList();

            Console.WriteLine($"Part2: {maximums.Max(maximum => maximum.Item1 + maximum.Item2)} in {watch.ElapsedMilliseconds}ms");
        }

        static int solve(Dictionary<State, int> cache, List<int> toOpen, State currentState, int currentFlow)
        {
            //if (cache.TryGetValue(currentState, out int bestFlow) && bestFlow >= currentFlow)
            //{
            //    return currentFlow;
            //}

            //cache[currentState] = currentFlow;

            //we are trying to open all valves in toOpen
            //we have opened (and added the final score) for all valves with bits set in valves
            //we have to try each valve remaining in toOpen
            int best = currentFlow;
            //start with the closest valves
            foreach (var nextValve in toOpen /*.OrderBy(valve => allPairs[currentState.currentValve, valve]) */)
            {
                if (!IsBitSet(nextValve, currentState.valves))
                {
                    //try opening this one next
                    var timeToValve = allPairs[currentState.currentValve, nextValve];
                    if (timeToValve + currentState.time + 1 < MINUTES)
                    {
                        //we can reach AND open it
                        int nextFlow = currentFlow + (MINUTES - (timeToValve + currentState.time + 1)) * indexGraph[nextValve].flow;
                        var nextValves = SetBit(nextValve, currentState.valves);
                        var nextState = currentState with { currentValve = nextValve, time = currentState.time + timeToValve + 1, valves = nextValves };
                        int nextBest = solve(cache, toOpen, nextState, nextFlow);
                        best = Math.Max(best, nextBest);
                    }
                }
            }

            return best;
        }
        static bool IsBitSet(int bit, ulong state) => (state & (1UL << bit)) != 0;
        static ulong SetBit(int bit, ulong state) => state |= 1UL << bit;
        private static int[,] floydWarshall(Dictionary<int, (int flow, List<int> edges)> graph)
        {
            int[,] distance = new int[graph.Count, graph.Count];
            for (int i = 0; i < graph.Count; i++)
            for (int j = 0; j < graph.Count; j++)
                distance[i, j] = MAX;

            foreach (var key in graph.Keys)
            foreach (var edge in graph[key].edges)
                distance[key, edge] = 1;

            for (int k = 0; k < graph.Count; k++)
            for (int i = 0; i < graph.Count; i++)
            for (int j = 0; j < graph.Count; j++)
            {
                if (distance[i, k] + distance[k, j] < distance[i, j])
                    distance[i, j] = distance[i, k] + distance[k, j];
            }

            return distance;
        }
    }
}