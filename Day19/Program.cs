using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day19
{
    internal class Program
    {
        record struct Material(int ore, int clay, int obsidian, int geode)
        {
            public Material Add(Material other) => new Material(ore + other.ore, clay + other.clay, obsidian + other.obsidian, geode + other.geode);
            public Material Subtract(Material other) => new Material(ore - other.ore, clay - other.clay, obsidian - other.obsidian, geode - other.geode);
            public Material Multiply(int multiple) => new Material(ore * multiple, clay * multiple, obsidian * multiple, geode * multiple);
        }

        record struct State(int time, Material material, Material production);
        
        static List<List<(Material costs, Material produces)>> blueprintSets;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            blueprintSets = File.ReadAllLines($"{args[0]}.txt")
                .Select(str => str.Split(' '))
                .Select(    arr => new List<(Material costs, Material production)>() {
                                     (new Material(int.Parse(arr[6]), 0, 0, 0), new Material(1,0,0,0)),
                                     (new Material(int.Parse(arr[12]), 0, 0, 0), new Material(0,1,0,0)),
                                     (new Material(int.Parse(arr[18]), int.Parse(arr[21]), 0, 0), new Material(0,0,1,0)),
                                     (new Material(int.Parse(arr[27]),0, int.Parse(arr[30]), 0), new Material(0,0,0,1)),
                            }).ToList();

            var part1Answers = RunPart(24, blueprintSets);
            var part1 = part1Answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, elapsed {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            var part2Answers = RunPart(32, blueprintSets.Take(3));
            var part2 = part2Answers.Select(tp => tp.Item2).Aggregate(1, (int acc, int a) => acc * a);            
            Console.WriteLine($"Part 2: {part2}, elapsed {watch.ElapsedMilliseconds}ms");
        }

        static List<(int index, int geodes)> RunPart(int MAXMINUTES, IEnumerable<List<(Material costs, Material produces)>> blueprintSets)
        {
            var answers = new List<(int, int)>();
            foreach ((var blueprints, int i) in blueprintSets.Select((bp, i) => (bp, i)))
            {
                var answer = Solve(MAXMINUTES, new Dictionary<State, int>(), blueprints, new State(1, new Material(0, 0, 0, 0), new Material(1, 0, 0, 0)));
                Console.WriteLine($"Blueprint {i}: {answer}");
                answers.Add((i + 1, answer));
            }

            return answers;
        }

        static int Solve(int MAXMINUTES, Dictionary<State, int> stateCache, List<(Material costs, Material production)> blueprints, State state)
        {
            if (stateCache.TryGetValue(state, out int result))
            {
                return result;
            }

            int maxGeode = state.material.geode;

            //we are only interested in testing our next ACTION
            //which is always to build a robot
            foreach (var blueprint in blueprints)
            {
                //find the next time we will be able to build this robot
                //which will be when we have accumulated sufficient material
                var timeForOre = Math.Ceiling((decimal)(blueprint.costs.ore - state.material.ore) / (state.production.ore == 0 ? 0.001m  : state.production.ore));
                var timeForClay = Math.Ceiling((decimal)(blueprint.costs.clay - state.material.clay) / (state.production.clay == 0 ? 0.001m : state.production.clay));
                var timeForObsidian = Math.Ceiling((decimal)(blueprint.costs.obsidian - state.material.obsidian) / (state.production.obsidian == 0 ? 0.001m : state.production.obsidian));

                var nextTime = (int) Math.Max(Math.Max(timeForOre, timeForClay), timeForObsidian);

                //any we can build immediately have already been dealt with
                //so only consider FUTURE robots
                if (nextTime > 0 && state.time + nextTime < MAXMINUTES)
                {
                    maxGeode = Math.Max(maxGeode, Solve(MAXMINUTES, stateCache, blueprints, 
                        state with { 
                            time = state.time + nextTime, 
                            material = state.material.Add(state.production.Multiply(nextTime)).Subtract(blueprint.costs), 
                            production = blueprint.production.geode == 0 
                                ? state.production.Add(blueprint.production)
                                //if building a geode we need to add the full score on immediately
                                : state.production.Add(blueprint.production.Multiply(MAXMINUTES - state.time - nextTime))
                        }));
                }
            }
                       
            stateCache[state] = maxGeode;
            return maxGeode;
        }
    }
}