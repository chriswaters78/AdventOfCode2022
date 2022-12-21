using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day19
{
    internal class Program
    {
        record struct Material(int ore, int clay, int obsidian)
        {
            public Material Add(Material other) => new Material(ore + other.ore, clay + other.clay, obsidian + other.obsidian);
            public Material Subtract(Material other) => new Material(ore - other.ore, clay - other.clay, obsidian - other.obsidian);
            public Material Multiply(int multiple) => new Material(ore * multiple, clay * multiple, obsidian * multiple);
        }
        
        record struct Blueprint(Material costs, Material production, bool isGeode);

        record struct State(int time, Material material, Material production, int currentScore, int best);
        record struct BlueprintSet(List<Blueprint> blueprints, Material maxCosts);

        static List<BlueprintSet> blueprintSets;

        static int best;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            blueprintSets = File.ReadAllLines($"{args[0]}.txt")
                .Select(str => str.Split(' '))
                .Select(    arr => new List<Blueprint>() {
                                     new Blueprint(new Material(int.Parse(arr[27]),0, int.Parse(arr[30])), new Material(0,0,0), true),
                                     new Blueprint(new Material(int.Parse(arr[18]), int.Parse(arr[21]), 0), new Material(0,0,1), false),
                                     new Blueprint(new Material(int.Parse(arr[12]), 0, 0), new Material(0,1,0), false),
                                     new Blueprint(new Material(int.Parse(arr[6]), 0, 0), new Material(1,0,0), false),
                            })
                .Select(bps => new BlueprintSet(bps, new Material(bps.Max(bp => bp.costs.ore), bps.Max(bp => bp.costs.clay), bps.Max(bp => bp.costs.obsidian))))
                .ToList();

            var part1Answers = RunPart(24, blueprintSets);
            var part1 = part1Answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, elapsed {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            var part2Answers = RunPart(32, blueprintSets.Take(3));
            var part2 = part2Answers.Select(tp => tp.Item2).Aggregate(1, (int acc, int a) => acc * a);            
            Console.WriteLine($"Part 2: {part2}, elapsed {watch.ElapsedMilliseconds}ms");
        }

        static List<(int index, int geodes)> RunPart(int MAXMINUTES, IEnumerable<BlueprintSet> blueprintSets)
        {
            var answers = new List<(int, int)>();
            foreach ((var blueprintSet, int i) in blueprintSets.Select((bp, i) => (bp, i)))
            {
                best = 0;
                //note we start at t=2 with 1 ore
                var answer = Solve(MAXMINUTES, new Dictionary<State, int>(), blueprintSet, new State(2, new Material(1, 0, 0), new Material(1, 0, 0), 0));
                Console.WriteLine($"Blueprint {i + 1}: {answer}");
                answers.Add((i + 1, answer));
            }

            return answers;
        }

        static int Solve(int MAXMINUTES, Dictionary<State, int> stateCache, BlueprintSet blueprintSet, State state)
        {
            if (stateCache.TryGetValue(state, out int result))
            {
                return result;
            }

            //e.g 4 minutes left, best we can do is build one geode-bot to score next minute and each minute after
            int bestFromHere = (MAXMINUTES - state.time) * (MAXMINUTES - state.time + 1) / 2;
            if (bestFromHere + state.currentScore <= best)
            {
                return 0;
            }

            int maxGeode = state.currentScore;

            //we are only interested in testing our next ACTION
            //which is always to build a robot
            foreach (var blueprint in blueprintSet.blueprints)
            {
                //we have ALWAYS built a robot last turn, the production reflects this
                //the material is what we have at the start of the minute
                //so minimum time until we can build next robot is next turn, or the first turn when we have sufficient material

                if ((state.production.ore >= blueprintSet.maxCosts.ore && blueprint.production.ore > 0)
                    || (state.production.clay >= blueprintSet.maxCosts.clay && blueprint.production.clay > 0)
                    || (state.production.obsidian >= blueprintSet.maxCosts.obsidian && blueprint.production.obsidian > 0))
                {
                    continue;
                }

                var needed = blueprint.costs.Subtract(state.material);

                var timeForOre = Math.Ceiling((decimal)needed.ore / (state.production.ore == 0 ? 0.001m : (decimal) state.production.ore));
                var timeForClay = Math.Ceiling((decimal)needed.clay / (state.production.clay == 0 ? 0.001m : (decimal)state.production.clay));
                var timeForObsidian = Math.Ceiling((decimal)needed.obsidian / (state.production.obsidian == 0 ? 0.001m : (decimal)state.production.obsidian));

                var nextTime = (int) Math.Max(0,Math.Max(Math.Max(timeForOre, timeForClay), timeForObsidian));

                if (state.time + nextTime < MAXMINUTES)
                {
                    maxGeode = Math.Max(maxGeode, Solve(MAXMINUTES, stateCache, blueprintSet,
                        state with { 
                            time = state.time + nextTime + 1, 
                            material = state.material.Add(state.production).Add(state.production.Multiply(nextTime)).Subtract(blueprint.costs), 
                            production = state.production.Add(blueprint.production),
                            currentScore = state.currentScore + (blueprint.isGeode ? (MAXMINUTES - state.time - nextTime) : 0),
                        }));
                }
            }

            stateCache[state] = maxGeode;
            best = Math.Max(best, maxGeode);
            return maxGeode;
        }
    }
}