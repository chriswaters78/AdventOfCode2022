using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day19
{
    internal class Program
    {
        static int MINUTES;
        //ore, clay, obsidian, geode
        static List<List<((int ore, int clay, int obsidian) costs, (int ore, int clay, int obsidian, int geode) produces)>> blueprints;
        static Dictionary<(int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production), int> state;
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            blueprints = File.ReadAllLines($"{args[0]}.txt")
                .Select(str => str.Split(' '))
                .Select(
                    arr =>
                     new List<((int ore, int clay, int obsidian) costs, (int ore, int clay, int obsidian, int geode) produces)>() {
                         ((int.Parse(arr[6]), 0,0), (1,0,0,0)),
                         ((int.Parse(arr[12]), 0,0), (0,1,0,0)),
                         ((int.Parse(arr[18]), int.Parse(arr[21]),0), (0,0,1,0)),
                         ((int.Parse(arr[27]),0, int.Parse(arr[30])), (0,0,0,1)),
                     }).ToList();

            MINUTES = 24;
            List<(int, int)> answers = new List<(int, int)>();
            for (int i = 1; i <= blueprints.Count; i++)
            {
                state = new Dictionary<(int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production), int>();
                var answer = solve(i - 1, 1, (0, 0, 0, 0), (1, 0, 0, 0));
                Console.WriteLine($"Blueprint {i}: {answer}, state count {state.Count} in {watch.ElapsedMilliseconds}ms");
                answers.Add((i, answer));
            }

            var part1 = answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, elapsed {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            MINUTES = 32;
            answers = new List<(int, int)>();            
            for (int i = 1; i <= 3; i++)
            {
                state = new Dictionary<(int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production), int>();
                var answer = solve(i - 1, 1, (0, 0, 0, 0), (1, 0, 0, 0));
                Console.WriteLine($"Blueprint {i}: {answer}, state count {state.Count} in {watch.ElapsedMilliseconds}ms");
                answers.Add((i, answer));
            }

            var part2 = answers.Select(tp => tp.Item2).Aggregate(1, (int acc, int a) => acc * a);
            Console.WriteLine($"Part 2: {part2}, elapsed {watch.ElapsedMilliseconds}ms");
        }

        static int solve(int bi, int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production)
        {
            if (state.ContainsKey((time, materials, production)))
            {
                return state[(time, materials, production)];
            }
            
            (int ore, int clay, int obsidian, int geode) materialsNext = (materials.ore + production.ore, materials.clay + production.clay, materials.obsidian + production.obsidian, materials.geode + production.geode);
            if (time == MINUTES)
            {
                return materialsNext.geode;
            }

            int maxGeode = 0;
            foreach (var blueprint in blueprints[bi])
            {
                if (materials.ore >= blueprint.costs.ore && materials.clay >= blueprint.costs.clay && materials.obsidian >= blueprint.costs.obsidian)
                {
                    maxGeode = Math.Max(maxGeode, 
                        solve(bi, time + 1,
                        (   materialsNext.ore - blueprint.costs.ore, 
                            materialsNext.clay - blueprint.costs.clay, 
                            materialsNext.obsidian - blueprint.costs.obsidian, 
                            materialsNext.geode), 
                        (   production.ore + blueprint.produces.ore, 
                            production.clay + blueprint.produces.clay, 
                            production.obsidian + blueprint.produces.obsidian, 
                            production.geode + blueprint.produces.geode)));
                }
            }

            maxGeode = Math.Max(maxGeode, 
                solve(bi, time + 1, materialsNext, production));
            
            state[(time, materials, production)] = maxGeode;
            return maxGeode;
        }
    }
}