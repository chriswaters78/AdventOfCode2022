using System.Diagnostics;

namespace Day16
{
    internal class Program
    {
        record struct State(int time, int currentValve, int currentFlow, uint toOpen);

        const int MINUTES = 26;
        const int MAX = 10000;

        static int nonZeroCount;        
        static Dictionary<int, (int flow, List<int> edges)> indexGraph;
        static int[,] allPairs;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //parse graph and assign indexes to each valve
            //making sure all the valves that can be opened are assigned an index first
            var stringGraph = File.ReadAllLines($"{args[0]}.txt")
                .Select(    str => (str[6..8],
                            int.Parse(str[23..25].Trim(';')),
                            (str.Contains("valves") ? str.Split("valves ")[1] : str.Split("valve ")[1]).Split(", ").ToArray()))
                .ToDictionary(tp => tp.Item1, tp => (tp.Item2, tp.Item3));

            nonZeroCount = stringGraph.Where(kvp => kvp.Value.Item1 != 0).Count();
            var stringToIndex = stringGraph.OrderByDescending(kvp => kvp.Value.Item1).Select((kvp, i) => (kvp.Key, i)).ToDictionary(tp => tp.Key, tp => tp.i);
            indexGraph = stringGraph.ToDictionary(kvp => stringToIndex[kvp.Key], kvp => (kvp.Value.Item1, kvp.Value.Item2.Select(str => stringToIndex[str]).ToList()));

            //get all pairs shortest paths using Floyd-Warshall
            //note this is the DISTANCE only so still need to take into account time to open the valve
            allPairs = floydWarshall(indexGraph);

            //partition the non-zero valves into all possible disjoint sets
            //skipping 1 bit to ensure that we don't count both (A,B) and (B,A)
            var sets = new List<(uint set1, uint set2)>();
            for (uint i = 0; i < Math.Pow(2, nonZeroCount - 1); i++)
            {
                (var set1, var set2) = (0U, 0U);                
                for (int b = 0; b < nonZeroCount; b++)
                    if (IsBitSet(b, i))
                        set1 = SetBit(b, set1);
                    else
                        set2 = SetBit(b, set2);
                sets.Add((set1, set2));
            }

            //for each pair of disjoint sets, recursively DFS each side starting at valve AA
            //at each level of the search, we only need to consider moving to each valve which remains open
            //Find the max of the sum of the pressure released for all disjoint pairs
            var maximums = sets
                .AsParallel()
                .Select(pair => (Solve(pair.set1, stringToIndex["AA"]), Solve(pair.set2, stringToIndex["AA"]))).ToList();

            Console.WriteLine($"Part2: {maximums.Max(maximum => maximum.Item1 + maximum.Item2)} in {watch.ElapsedMilliseconds}ms");
        }

        static int Solve(uint toOpen, int startValve)
        {
            int currentBest = 0;
            return solve(new Dictionary<State, int>(), ref currentBest, new State(0, startValve, 0, toOpen));
        }

        static int solve(Dictionary<State, int> cache, ref int currentBest, State state)
        {
            if (cache.TryGetValue(state, out int result))
            {
                return result;
            }

            //we need a upper limit for the maximum we can achieve from this point
            //which we calculate by assuming all remaining valves which it is still possible to open
            //can be reached in the min time to that valve

            var queue = new PriorityQueue<int, int>();
            int maxFlow = 0;
            for (int b = 0; b < nonZeroCount; b++)
                if (IsBitSet(b, state.toOpen))
                    if (allPairs[state.currentValve, b] + state.time + 1 < MINUTES)
                    {
                        var additionalFlow = (MINUTES - (allPairs[state.currentValve, b] + state.time + 1)) * indexGraph[b].flow;
                        maxFlow += additionalFlow;
                        queue.Enqueue(b, -additionalFlow);
                    }
                    else
                    {
                        //no longer possible to open this valve so remove from set
                        state.toOpen = ClearBit(b, state.toOpen);
                    }

            if (state.currentFlow + maxFlow <= currentBest)
                return 0;

            //we are trying to open all valves in toOpen which haven't been opened yet
            //we have opened (and added the final score) already all valves opened to this point
            int best = state.currentFlow;
            while (queue.TryDequeue(out int nextValve, out int negAdditionalFlow))
            {
                var newState = state with
                {
                    time = allPairs[state.currentValve, nextValve] + state.time + 1,
                    currentFlow = state.currentFlow - negAdditionalFlow,
                    currentValve = nextValve,
                    toOpen = ClearBit(nextValve, state.toOpen)
                };

                int nextBest = solve(cache, ref currentBest, newState);
                best = Math.Max(best, nextBest);
                currentBest = Math.Max(best, currentBest);
            }

            cache[state] = best;
            return best;
        }
        static bool IsBitSet(int bit, uint state) => (state & (1U << bit)) != 0;
        static uint SetBit(int bit, uint state) => state |= 1U << bit;
        static uint ClearBit(int bit, uint state) => state &= ~(1U << bit);
        
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