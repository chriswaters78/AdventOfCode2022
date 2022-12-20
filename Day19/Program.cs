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

        record struct State(int time, Material material, Material production, Material lastProduced, int currentScore);
        
        static List<List<(Material costs, Material produces, bool isGeode)>> blueprintSets;

        static int best;

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            blueprintSets = File.ReadAllLines($"{args[0]}.txt")
                .Select(str => str.Split(' '))
                .Select(    arr => new List<(Material costs, Material production, bool isGeode)>() {
                                     (new Material(int.Parse(arr[6]), 0, 0), new Material(1,0,0), false),
                                     (new Material(int.Parse(arr[12]), 0, 0), new Material(0,1,0), false),
                                     (new Material(int.Parse(arr[18]), int.Parse(arr[21]), 0), new Material(0,0,1), false),
                                     (new Material(int.Parse(arr[27]),0, int.Parse(arr[30])), new Material(0,0,0), true),
                            }).ToList();

            var part1Answers = RunPart(24, blueprintSets);
            var part1 = part1Answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, elapsed {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            var part2Answers = RunPart(32, blueprintSets.Take(3));
            var part2 = part2Answers.Select(tp => tp.Item2).Aggregate(1, (int acc, int a) => acc * a);            
            Console.WriteLine($"Part 2: {part2}, elapsed {watch.ElapsedMilliseconds}ms");
        }

        static List<(int index, int geodes)> RunPart(int MAXMINUTES, IEnumerable<List<(Material costs, Material produces, bool isGeode)>> blueprintSets)
        {
            best = 0;
            var answers = new List<(int, int)>();
            foreach ((var blueprints, int i) in blueprintSets.Select((bp, i) => (bp, i)))
            {
                var answer = Solve(MAXMINUTES, new Dictionary<State, int>(), blueprints, new State(1, new Material(0, 0, 0), new Material(1, 0, 0), new Material(0, 0, 0), 0));
                Console.WriteLine($"Blueprint {i}: {answer}");
                answers.Add((i + 1, answer));
            }

            return answers;
        }

        static int Solve(int MAXMINUTES, Dictionary<State, int> stateCache, List<(Material costs, Material production, bool isGeode)> blueprints, State state)
        {
            if (stateCache.TryGetValue(state, out int result))
            {
                return result;
            }

            //state.material is the material we have at the start of this minute (after building whatever robot we build this minute)
            //state.production is the production we will have THIS MINUTE
            //state.currentScore is the final value (at MAXMINUTES of any geodes we have already built
            
            int maxGeode = state.currentScore;

            //we are only interested in testing our next ACTION
            //which is always to build a robot
            foreach (var blueprint in blueprints)
            {
                //we have ALWAYS built a robot this turn (except the first turn)
                //so minimum time until we can build next robot is next turn, or the first turn when we have sufficient material
                var timeForOre = Math.Ceiling((blueprint.costs.ore - state.material.ore) / (state.production.ore == 0 ? 0.001m  : state.production.ore));
                var timeForClay = Math.Ceiling((blueprint.costs.clay - state.material.clay) / (state.production.clay == 0 ? 0.001m : state.production.clay));
                var timeForObsidian = Math.Ceiling((blueprint.costs.obsidian - state.material.obsidian) / (state.production.obsidian == 0 ? 0.001m : state.production.obsidian));

                if (timeForOre <= 0) timeForOre = 1;
                if (timeForClay <= 0) timeForClay = 1;
                if (timeForObsidian <= 0) timeForObsidian = 1;

                var nextTime = (int) Math.Max(Math.Max(timeForOre, timeForClay), timeForObsidian);

                //any we can build immediately have already been dealt with
                //so only consider FUTURE robots
                if (nextTime > 0 && state.time + nextTime < MAXMINUTES)
                {
                    maxGeode = Math.Max(maxGeode, Solve(MAXMINUTES, stateCache, blueprints, 
                        state with { 
                            time = state.time + nextTime, 
                            material = state.material.Add(state.production.Multiply(nextTime)).Subtract(blueprint.costs), 
                            production = state.production.Add(state.lastProduced),
                            currentScore = state.currentScore + (blueprint.isGeode ? (MAXMINUTES - state.time - nextTime) : 0),
                            lastProduced = blueprint.production
                        }));
                }
            }

            if (maxGeode > best)
            {
                best = maxGeode;
            }
            stateCache[state] = maxGeode;
            return maxGeode;
        }
    }
}