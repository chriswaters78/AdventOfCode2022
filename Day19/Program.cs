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

        record struct State(int time, Material material, Material production, int currentScore, Blueprint toBuild);
        
        static List<List<Blueprint>> blueprintSets;

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
                            }).ToList();

            var part1Answers = RunPart(24, blueprintSets);
            var part1 = part1Answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, elapsed {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            var part2Answers = RunPart(32, blueprintSets.Take(3));
            var part2 = part2Answers.Select(tp => tp.Item2).Aggregate(1, (int acc, int a) => acc * a);            
            Console.WriteLine($"Part 2: {part2}, elapsed {watch.ElapsedMilliseconds}ms");
        }

        static List<(int index, int geodes)> RunPart(int MAXMINUTES, IEnumerable<List<Blueprint>> blueprintSets)
        {
            best = 0;
            var answers = new List<(int, int)>();
            var emptyBlueprint = new Blueprint(new Material(0, 0, 0), new Material(0, 0, 0), false);
            foreach ((var blueprints, int i) in blueprintSets.Select((bp, i) => (bp, i)))
            {
                //note we start with 1 ore, as this is the material AFTER the minute being considered
                var answer = Solve(MAXMINUTES, new Dictionary<State, int>(), blueprints, new State(1, new Material(0, 0, 0), new Material(1, 0, 0), 0, emptyBlueprint));
                Console.WriteLine($"Blueprint {i + 1}: {answer}");
                answers.Add((i + 1, answer));
            }

            return answers;
        }

        static int Solve(int MAXMINUTES, Dictionary<State, int> stateCache, List<Blueprint> blueprints, State state)
        {
            if (stateCache.TryGetValue(state, out int result))
            {
                return result;
            }
            
            int maxGeode = state.currentScore;

            //we are only interested in testing our next ACTION
            //which is always to build a robot
            foreach (var blueprint in blueprints)
            {
                //we have ALWAYS built a robot this turn (except the first turn)
                //so minimum time until we can build next robot is next turn, or the first turn when we have sufficient material

                //e.g. we have 1 ore, with 1 ore production (next turn) and it costs 4 ore to build
                var needed = blueprint.costs.Subtract(state.material).Add(state.toBuild.costs);
                var neededNextMinute = needed.Subtract(state.production);
                var productionNextMinute = state.production.Add(state.toBuild.production);
                
                var timeForOre = needed.ore <= 0 ? 1 : productionNextMinute.ore == 0 ? MAXMINUTES :
                    Math.Ceiling((decimal)neededNextMinute.ore / productionNextMinute.ore) + 1;

                var timeForClay = needed.clay <= 0 ? 1 : productionNextMinute.clay == 0 ? MAXMINUTES :
                    Math.Ceiling((decimal)neededNextMinute.clay / productionNextMinute.clay) + 1;

                var timeForObsidian = needed.obsidian <= 0 ? 1 : productionNextMinute.obsidian == 0 ? MAXMINUTES :
                    Math.Ceiling((decimal)neededNextMinute.obsidian / productionNextMinute.obsidian) + 1;

                var nextTime = (int) Math.Max(Math.Max(timeForOre, timeForClay), timeForObsidian);

                //state.material is the material we have at the start of this minute
                //toBuild is the blueprint we will build
                //state.production is the existing production we have
                //state.currentScore is the final value (at MAXMINUTES) of any geodes we have already built
                if (state.time + nextTime < MAXMINUTES)
                {
                    maxGeode = Math.Max(maxGeode, Solve(MAXMINUTES, stateCache, blueprints, 
                        state with { 
                            time = state.time + nextTime, 
                            material = state.material.Add(state.production).Add(state.production.Add(state.toBuild.production).Multiply(nextTime - 1)).Subtract(state.toBuild.costs), 
                            production = state.production.Add(state.toBuild.production),
                            currentScore = state.currentScore + (blueprint.isGeode ? (MAXMINUTES - state.time - nextTime) : 0),
                            toBuild = blueprint
                        }));
                }
            }

            stateCache[state] = maxGeode;
            return maxGeode;
        }
    }
}