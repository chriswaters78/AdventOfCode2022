namespace Day8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var grid = File.ReadAllLines("input.txt").Select(line => line.Select(c => int.Parse(c.ToString())).ToArray()).ToArray();

            int R = grid.Length;
            int C = grid.First().Length;


            Dictionary<(int r, int c), int> heightMap = grid.SelectMany((row, ri) => row.Select((h, ci) => (ri, ci, h))).ToDictionary(t => (t.ri, t.ci), t => t.h);

            int part1 = 0;
            int part2 = 0;
            for (int r = 0; r < R; r++)
            {
                for (int c = 0; c < C; c++)
                {
                    var vU = heightMap.Where(kvp => kvp.Key.c == c && kvp.Key.r < r).All(kvp => kvp.Value < heightMap[(r, c)]);
                    var vD = heightMap.Where(kvp => kvp.Key.c == c && kvp.Key.r > r).All(kvp => kvp.Value < heightMap[(r, c)]);
                    var vL = heightMap.Where(kvp => kvp.Key.r == r && kvp.Key.c < c).All(kvp => kvp.Value < heightMap[(r, c)]);
                    var vR = heightMap.Where(kvp => kvp.Key.r == r && kvp.Key.c > c).All(kvp => kvp.Value < heightMap[(r, c)]);
                    if (vU || vD || vL || vR)
                    {
                        part1++;
                    }

                    int sU = 0;
                    for (int rx = r - 1; rx >= 0; rx--)
                    {
                        sU++;
                        if (heightMap[(rx, c)] >= heightMap[(r, c)])
                            break;
                    }
                    int sD = 0;
                    for (int rx = r + 1; rx < R; rx++)
                    {
                        sD++;
                        if (heightMap[(rx, c)] >= heightMap[(r, c)])
                            break;
                    }
                    int sL = 0;
                    for (int cx = c - 1; cx >= 0; cx--)
                    {
                        sL++;
                        if (heightMap[(r, cx)] >= heightMap[(r, c)])
                            break;
                    }
                    int sR = 0;
                    for (int cx = c + 1; cx < C; cx++)
                    {
                        sR++;
                        if (heightMap[(r, cx)] >= heightMap[(r, c)])
                            break;
                    }
                     
                    part2 = Math.Max(part2, sU * sD * sL * sR);
                }
            }

        }
    }
}