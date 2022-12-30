using System.Diagnostics;

namespace Day16
{
    internal class Program
    {
        const int MINUTES = 26;
        const int MAX = 10000;
        
        static Dictionary<int, (int flow, List<int> edges)> indexGraph;
        static int[,] allPairs;

        record struct State(int time, uint valves, int currentValve);

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

            //get all pairs shortest paths using Floyd-Warshall
            //note this is the DISTANCE only so still need to take into account time to open the valve
            allPairs = floydWarshall(indexGraph);

            //partition our valve sets into disjoint sets
            //skip 1 bit to ensure that we don't count both (A,B) and (B,A)
            var sets = new List<(int[] set1, int[] set2)>();
            for (uint i = 0; i < Math.Pow(2, nonZeroCount - 1); i++)
            {
                (var list1, var list2) = (new List<int>(), new List<int>());
                for (int b = 0; b < nonZeroCount; b++)
                {
                    if (IsBitSet(b, i))
                        list1.Add(b);
                    else
                        list2.Add(b);
                }
                sets.Add((list1.ToArray(), list2.ToArray()));
            }

            Console.WriteLine($"Starting solve after {watch.ElapsedMilliseconds}ms");
            //then for each disjoint item, recursively DFS each side of the set, starting at AA
            //and find the maximum pressure relieved within the time limit
            //the maximum of all of these is the answer

            var maximums = sets.AsParallel().Select(pair => (Solve(pair.set1, stringToIndex["AA"]), Solve(pair.set2, stringToIndex["AA"]))).ToList();

            Console.WriteLine($"Part2: {maximums.Max(maximum => maximum.Item1 + maximum.Item2)} in {watch.ElapsedMilliseconds}ms");
        }

        static int Solve(int[] toOpen, int startValve)
        {
            int currentBest = 0;
            return solve(new Dictionary<State, int>(), toOpen, ref currentBest, new State(0, 0, startValve), 0);
        }

        static int solve(Dictionary<State, int> cache, int[] toOpen, ref int currentBest, State currentState, int currentFlow)
        {
            if (cache.TryGetValue(currentState, out int bestFlow) && bestFlow >= currentFlow)
            {
                return currentFlow;
            }
            cache[currentState] = currentFlow;

            //we need a upper limit for the maximum we can achieve from this point
            //which we calculate by assuming all remaining valves can be reached in the min time to that valve
            var canStillOpen = toOpen.Where(i => !IsBitSet(i, currentState.valves)
                && allPairs[currentState.currentValve, i] + currentState.time + 1 < MINUTES)
                .OrderBy(i => allPairs[currentState.currentValve, i]).ToArray();

            var maxFlow = canStillOpen.Select(i => (MINUTES - (allPairs[currentState.currentValve, i] + currentState.time + 1)) * indexGraph[i].flow).Sum();
            if (currentFlow + maxFlow <= currentBest)
            {
                return 0;
            }

            //we are trying to open all valves in toOpen which haven't been opened yet
            //we have opened (and added the final score) already all valves opened to this point
            int best = currentFlow;
            foreach (var nextValve in canStillOpen)
            {
                var timeToValve = allPairs[currentState.currentValve, nextValve];
                //we can reach AND open it
                int nextFlow = currentFlow + (MINUTES - (timeToValve + currentState.time + 1)) * indexGraph[nextValve].flow;
                var nextState = currentState with { currentValve = nextValve, time = currentState.time + timeToValve + 1, valves = SetBit(nextValve, currentState.valves) };                        
                int nextBest = solve(cache, canStillOpen, ref currentBest, nextState, nextFlow);
                best = Math.Max(best, nextBest);
                currentBest = Math.Max(best, currentBest);
            }

            return best;
        }
        static bool IsBitSet(int bit, uint state) => (state & (1U << bit)) != 0;
        static uint SetBit(int bit, uint state) => state |= 1U << bit;
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