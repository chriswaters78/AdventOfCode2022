using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml.Linq;

namespace Day16
{   
    internal class Program
    {
        static Dictionary<(string node, int timespent, string valves), int> state;
        static Dictionary<(int node1, int node2, int timespent, ulong valves), int> state2;
        static Dictionary<string, (int flow, string[] edges)> graph;
        static Dictionary<int, (int flow, int[] edges)> graph2;
        static int best = 0;
        static void Main(string[] args)
        {
            var graph = File.ReadAllLines($"{args[0]}.txt")
                .Select(
                        str => (str[6..8], 
                        int.Parse(str[23..25].Trim(';')),
                        (str.Contains("valves") ? str.Split("valves ")[1] : str.Split("valve ")[1]).Split(", ").ToArray()
                    )).ToDictionary(tp => tp.Item1, tp => (tp.Item2, tp.Item3));

            //define our state by (location, timespent, valvesopened)
            state = new Dictionary<(string, int, string), int>();
            state2 = new Dictionary<(int, int, int, ulong), int>();


            var nodeIndexes = graph.Keys.Select((str, i) => (str, i)).ToDictionary(tp => tp.Item1, tp => tp.Item2);
            graph2 = graph.ToDictionary(kvp => nodeIndexes[kvp.Key], kvp => (kvp.Value.Item1, kvp.Value.Item2.Select(str => nodeIndexes[str]).ToArray()));

            //solve(0, 0, "", "AA");
            //var part1 = state.Max(kvp => kvp.Value);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            solve2(0, 0, 0, nodeIndexes["AA"], nodeIndexes["AA"]);
            var part2 = state2.Max(kvp => kvp.Value);
            Console.WriteLine($"Part2: {part2} in {watch.ElapsedMilliseconds}ms");
        }

        static bool IsBitSet(int bit, ulong state)
        {
            return (state & (1UL << bit)) != 0;
        }
        static ulong SetBit(int bit, ulong state)
        {
            return state |= 1UL << bit;
        }

        static int maxFlowLeft(ulong state)
        {
            int flow = 0;
            for (int i = 0; i < graph2.Count; i++)
            {
                if (!IsBitSet(i, state))
                {
                    flow += graph2[i].flow;
                }
            }

            return flow;
        }
        
        static void solve2(int timespent, int currentflow, ulong valves, int node1, int node2)
        {
            const int MINUTES = 26;

            best = Math.Max(currentflow, best);
            
            if (maxFlowLeft(valves) * (MINUTES - timespent) + currentflow < best)
            {
                //not possible to improve from here
                 return;
            }

            //sort these, as elephant and man are completely equivalent so order doesn't matter
            if (node2 < node1)
            {
                (node1, node2) = (node2, node1);
            }
            if (state2.TryGetValue((node1, node2, timespent, valves), out int value)
                && value >= currentflow)
            {
                //we've already been here in the same time and done better
                return;
            }

            state2[(node1, node2, timespent, valves)] = currentflow;
            if (timespent >= MINUTES)
            {
                return;
            }

            //we have the following options that can happen in the same minute
            // openE, openM  -- not possible if both in same  location
            // moveE, openM
            // openE, moveM
            // moveE, moveM

            bool canOpen1 = !IsBitSet(node1, valves) && graph2[node1].flow > 0;
            bool canOpen2 = !IsBitSet(node2, valves) && graph2[node2].flow > 0;            

            //both can open
            if (node1 != node2 && canOpen1 && canOpen2)
            {
                var newFlow = currentflow + (MINUTES - (timespent + 1)) * graph2[node1].flow + (MINUTES - (timespent + 1)) * graph2[node2].flow;
                var newValves = SetBit(node1, SetBit(node2,valves));
                solve2(timespent + 1, newFlow, newValves, node1, node2);
            }
            if (canOpen1)
            {
                var newFlow = currentflow + (MINUTES - (timespent + 1)) * graph2[node1].flow;
                var newValves = SetBit(node1, valves);
                foreach (var node in graph2[node2].edges)
                {
                    solve2(timespent + 1, newFlow, newValves, node1, node);
                }
            }
            if (canOpen2)
            {
                var newFlow = currentflow + (MINUTES - (timespent + 1)) * graph2[node2].flow;
                var newValves = SetBit(node2, valves);
                foreach (var node in graph2[node1].edges)
                {
                    solve2(timespent + 1, newFlow, newValves, node, node2);
                }
            }
            //both move
            foreach (var nodeE in graph2[node2].edges)
            {
                foreach (var nodeM in graph2[node1].edges)
                {
                    solve2(timespent + 1, currentflow, valves, nodeE, nodeM);
                }
            }
        }


        static void solve(int timespent, int currentflow, string valvesopen, string location)
        {
            if (state.ContainsKey((location, timespent, valvesopen)) && state[(location, timespent, valvesopen)] >= currentflow)
            {
                //we've already been here and we've already done better
                return;
            }

            state[(location, timespent, valvesopen)] = currentflow;
            if (timespent >= 30)
            {
                return;
            }

            if (!valvesopen.Contains(location) && timespent <= 29 && graph[location].flow > 0)
            {
                //we can open this valve
                (string location, int timespent, string valvesopen) newState =
                    (location, timespent + 1,
                        String.Join("-", valvesopen.Split('-', StringSplitOptions.RemoveEmptyEntries).Concat(new[] { location }).OrderBy(str => str).ToArray()));

                var newFlow = currentflow + (30 - newState.timespent) * graph[location].flow;

                if (!state.ContainsKey(newState) || state[newState] < newFlow)
                {
                    solve(newState.timespent, newFlow, newState.valvesopen, location);
                }
            }

            //now do the don't open valve version
            foreach (var node in graph[location].edges)
            {
                solve(timespent + 1, currentflow, valvesopen, node);
            }
        }


    }
}