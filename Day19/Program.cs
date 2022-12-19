using System.Collections.Generic;
using System.Linq;

namespace Day19
{
    internal class Program
    {
        const int MINUTES = 24;
        //ore, clay, obsidian, geode
        static List<List<((int ore, int clay, int obsidian) costs, (int ore, int clay, int obsidian, int geode) produces)>> blueprints;
        static Dictionary<(int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production), int> state;
        static void Main(string[] args)
        {
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

            List<(int, int)> answers = new List<(int, int)>();

            for (int i = 1; i <= blueprints.Count; i++)
            {
                Console.WriteLine($"Testing blue print {i}");
                best = 0;
                state = new Dictionary<(int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production), int>();

                var answer = solve(i - 1, 1, (0, 0, 0, 0), (1, 0, 0, 0));
                Console.WriteLine($"Best was {answer}");
                answers.Add((i, answer));
            }

            var part1 = answers.Sum(tp => tp.Item1 * tp.Item2);
            Console.WriteLine($"Part 1: {part1}, state count {state.Count}");
        }

        static int best;
        //return max geodes
        static int solve(int bi, int time, (int ore, int clay, int obsidian, int geode) materials, (int ore, int clay, int obsidian, int geode) production)
        {
            if (state.ContainsKey((time, materials, production)))
            {
                return state[(time, materials, production)];
            }
            
            int maxGeode = 0;
            (int ore, int clay, int obsidian, int geode) materialsNext = (materials.ore + production.ore, materials.clay + production.clay, materials.obsidian + production.obsidian, materials.geode + production.geode);
            if (time == MINUTES)
            {
                if (materialsNext.Item4 > best)
                {
                    Console.WriteLine($"New best {best}");
                    best = materialsNext.Item4;
                }
                return materialsNext.Item4;
            }

            foreach (var blueprint in blueprints[bi])
            {
                if (materials.ore >= blueprint.costs.ore && materials.clay >= blueprint.costs.clay && materials.obsidian >= blueprint.costs.obsidian)
                {
                    //we could build one
                    var productionNext = (production.ore + blueprint.produces.ore, production.clay + blueprint.produces.clay, production.obsidian + blueprint.produces.obsidian, production.geode + blueprint.produces.geode);
                    var materialsNextMinusCost = (materialsNext.ore - blueprint.costs.ore, materialsNext.clay - blueprint.costs.clay, materialsNext.obsidian - blueprint.costs.obsidian, materialsNext.geode);
                    maxGeode = Math.Max(maxGeode, solve(bi, time + 1, materialsNextMinusCost, productionNext));
                }
            }

            maxGeode = Math.Max(maxGeode, solve(bi, time + 1, materialsNext, production));

            state[(time, materials, production)] = maxGeode;
            return maxGeode;
        }
    }
}