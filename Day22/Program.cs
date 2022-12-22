using System.Text;

namespace Day22
{
    internal class Program
    {
        public enum Face
        {
            Top,
            Bottom,
            Left,
            Right,
            Front,
            Back
        }        

        static int CUBESIZE;
        static List<bool[][]> faces;
        static List<(int d, char turn)> moves;

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines($"{args[0]}.txt");
            CUBESIZE = (int) Math.Sqrt(lines.Take(lines.Length - 2).SelectMany(str => str).Count(ch => ch != ' ') / 6);

            var gridLines = lines.Take(lines.Length - 2).Select(line => line.Replace(" ", "")).ToArray();
            moves = new List<(int d, char turn)>();
            int i = 0;
            string val = "";
            var moveStr = lines.Last();
            while (i <= moveStr.Length)                
            {
                if (i < moveStr.Length  && Char.IsDigit(moveStr[i]))
                {
                    val += moveStr[i];
                }
                else
                {
                    moves.Add((int.Parse(val), i < moveStr.Length ? moveStr[i] : 'X'));
                    val = "";
                }
                i++;
            }

            faces = new List<bool[][]>();
            for (int l = 0; l < gridLines.Length / CUBESIZE; l++)
            {
                for (int w = 0; w < gridLines[l * CUBESIZE].Length / CUBESIZE; w++)
                {
                    var face = new bool[CUBESIZE][];
                    faces.Add(face);
                    for (int r = 0; r < CUBESIZE; r++)
                    {
                        face[r] = new bool[CUBESIZE];
                        for (int c = 0; c < CUBESIZE; c++)
                        {
                            face[r][c] = gridLines[l * CUBESIZE + r][w * CUBESIZE + c] == '#';
                        }
                    }
                }
            }

            var faceMapSample = new Dictionary<Face, bool[][]>()
            {
                { Face.Top, faces[0]},
                { Face.Back, faces[1]},
                { Face.Left, faces[2]},
                { Face.Front, faces[3]},
                { Face.Bottom, faces[4]},
                { Face.Right, faces[5]},
            };
            var faceIndexMapSample = new Dictionary<Face, (int r, int c)>()
            {
                { Face.Top, (0, 2)},
                { Face.Back, (1, 0)},
                { Face.Left, (1,1)},
                { Face.Front, (1,2)},
                { Face.Bottom, (2,2)},
                { Face.Right, (2,3)},
            };
            var faceMapInput = new Dictionary<Face, bool[][]>()
            {
                { Face.Top, faces[0]},
                { Face.Right, faces[1]},
                { Face.Front, faces[2]},
                { Face.Left, faces[3]},
                { Face.Bottom, faces[4]},
                { Face.Back, faces[5]},
            };
            var faceIndexMapInput = new Dictionary<Face, (int r, int c)>()
            {
                { Face.Top, (0, 1)},
                { Face.Right, (0, 2)},
                { Face.Front, (1,1)},
                { Face.Left, (2,0)},
                { Face.Bottom, (2,1)},
                { Face.Back, (3,0)},
            };

            var faceMap = args[0] == "test" ? faceMapSample : faceMapInput;
            var faceIndexMap = args[0] == "test" ? faceIndexMapSample : faceIndexMapInput;
            Func<Face, (int r, int c), (int dr, int dc), (Face face, (int r, int c) position, (int dr, int dc) direction)> transitionPart1 = args[0] == "test" ? transition1_sample : transition1_input;
            Func<Face, (int r, int c), (int dr, int dc), (Face face, (int r, int c) position, (int dr, int dc) direction)> transitionPart2 = args[0] == "test" ? transition2_sample : transition2_input;

            var part1 = solve(faceMap, faceIndexMap, transitionPart1);
            Console.WriteLine($"Part 1: {part1}");

            var part2 = solve(faceMap, faceIndexMap, transitionPart2);
            Console.WriteLine($"Part 2: {part2}");
        }

        static int solve(Dictionary<Face, bool[][]> faceMap, Dictionary<Face, (int r, int c)> faceIndexMap, Func<Face, (int r, int c), (int dr, int dc), (Face face, (int r, int c) position, (int dr, int dc) direction)> transition)
        {
            (Face face, (int r, int c) position, (int dr, int dc) direction) state = (Face.Top, (0, 0), (0, 1));
            foreach (var move in moves)
            {
                for (int m = 0; m < move.d; m++)
                {
                    (Face face, (int r, int c) position, (int dr, int dc) direction) nextPosition = (state.face, (state.position.r + state.direction.dr, state.position.c + state.direction.dc), state.direction);
                    nextPosition = transition(nextPosition.face, nextPosition.position, nextPosition.direction);
                    if (faceMap[nextPosition.face][nextPosition.position.r][nextPosition.position.c])
                    {
                        break;
                    }
                    state = nextPosition;
                }
                if (move.turn != 'X')
                {
                    if (state.direction.dc != 0)
                    {
                        state.direction = (move.turn == 'R' ? state.direction.dc : state.direction.dc * -1, 0);
                    }
                    else
                    {
                        state.direction = (0, move.turn == 'R' ? -1 * state.direction.dr : state.direction.dr);
                    }
                }

                //Console.WriteLine($"Moved {move.d} and turned {move.turn}");
                //print(faceMap[state.face], state.face, state.position, state.direction);
            }

            return (faceIndexMap[state.face].r * CUBESIZE + state.position.r + 1) * 1000 + (faceIndexMap[state.face].c * CUBESIZE + state.position.c + 1) * 4 + directionScore(state.direction);
        }

        private static int directionScore((int dr, int dc) direction) => direction switch
        {
            (0, 1) => 0,
            (1, 0) => 1,
            (0, -1) => 2,
            (-1, 0) => 3,
        };

        private static void print(bool[][] grid, Face face, (int r, int c) position, (int dr, int dc) direction)
        {
            List<StringBuilder> rows = new List<StringBuilder>();
            for (int r = 0; r < CUBESIZE; r++)
            {
                rows.Add(new StringBuilder());
                for (int c = 0; c < CUBESIZE; c++)
                {
                    rows[r].Append(grid[r][c] ? '#' : '.');
                }
            }

            switch (direction)
            {
                case (0, 1):
                    rows[position.r][position.c] = '>';
                    break;
                case (0, -1):
                    rows[position.r][position.c] = '<';
                    break;
                case (1, 0):
                    rows[position.r][position.c] = 'v';
                    break;
                case (-1, 0):
                    rows[position.r][position.c] = '^';
                    break;
            }

            Console.WriteLine($"Face: {face}, r: {position.r}, c: {position.c}");
            Console.WriteLine(String.Join(Environment.NewLine, rows.Select(sb => sb.ToString())));
        }


        private static (Face face, (int r, int c) position, (int dr, int dc) direction) transition2_input(Face face, (int r, int c) position, (int dr, int dc) direction)
        {
            switch (face)
            {
                case Face.Top:
                    if (position.c < 0)
                    {
                        return (Face.Left, (CUBESIZE - 1 - position.r, 0), (0, 1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Back, (position.c, 0), (0, 1));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Front, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Right:
                    if (position.c < 0)
                    {
                        return (Face.Top, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Bottom, (CUBESIZE - 1 - position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Back, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Front, (position.c, CUBESIZE - 1), (0, -1));
                    }
                    return (face, position, direction);
                case Face.Left:
                    if (position.c < 0)
                    {
                        return (Face.Top, (CUBESIZE - 1 - position.r, 0), (0, 1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Bottom, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (position.c, 0), (0, 1));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Back:
                    if (position.c < 0)
                    {
                        return (Face.Top, (0, position.r), (1, 0));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Bottom, (CUBESIZE - 1, position.r), (-1, 0));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Left, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Right, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Front:
                    if (position.c < 0)
                    {
                        return (Face.Left, (0, position.r), (1, 0));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (CUBESIZE - 1, position.r), (-1, 0));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                //bottom
                default:
                    if (position.c < 0)
                    {
                        return (Face.Left, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (CUBESIZE - 1 - position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (position.c, CUBESIZE - 1), (0, -1));
                    }
                    return (face, position, direction);
            }
        }


        private static (Face face, (int r, int c) position, (int dr, int dc) direction) transition2_sample(Face face, (int r, int c) position, (int dr, int dc) direction)
        {
            switch (face)
            {
                case Face.Top:
                    if (position.c < 0)
                    {
                        return (Face.Left, (0, position.r), (-1, 0));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (0, position.r), (-1, 0));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Back, (0, CUBESIZE - 1 - position.c), (1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Front, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Right:
                    if (position.c < 0)
                    {
                        return (Face.Bottom, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Top, (CUBESIZE - 1 - position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (CUBESIZE - 1 - position.c, CUBESIZE - 1), (0, -1));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (CUBESIZE - 1 - position.c, 0), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Left:
                    if (position.c < 0)
                    {
                        return (Face.Back, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Front, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (position.c, 0), (0, 1));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (CUBESIZE - 1 - position.c, 0), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Back:
                    if (position.c < 0)
                    {
                        return (Face.Right, (0, CUBESIZE - 1 - position.c), (1, 0));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Left, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (0, CUBESIZE - 1 - position.c), (1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    return (face, position, direction);
                case Face.Front:
                    if (position.c < 0)
                    {
                        return (Face.Left, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (0, CUBESIZE - 1 - position.r), (1, 0));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                //bottom
                default:
                    if (position.c < 0)
                    {
                        return (Face.Left, (CUBESIZE - 1, CUBESIZE - 1 - position.r), (-1, 0));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (CUBESIZE - 1, CUBESIZE - 1 - position.c), (-1, 0));
                    }
                    return (face, position, direction);
            }
        }


        private static (Face face, (int r, int c) position, (int dr, int dc) direction) transition1_input(Face face, (int r, int c) position, (int dr, int dc) direction)
        {
            switch (face)
            {
                case Face.Top:
                    if (position.c < 0)
                    {
                        return (Face.Right, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Bottom, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Front, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Right:
                    if (position.c < 0)
                    {
                        return (Face.Top, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Top, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Right, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Right, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Left:
                    if (position.c < 0)
                    {
                        return (Face.Bottom, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Bottom, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Back, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Back:
                    if (position.c < 0)
                    {
                        return (Face.Back, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Back, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Left, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Left, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Front:
                    if (position.c < 0)
                    {
                        return (Face.Front, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Front, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                //bottom
                default:
                    if (position.c < 0)
                    {
                        return (Face.Left, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Left, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Top, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
            }
        }

        private static (Face face, (int r, int c) position, (int dr, int dc) direction) transition1_sample(Face face, (int r, int c) position, (int dr, int dc) direction)
        {
            //we are on face
            //and r and c are our current position
            //check if we have moved OFF the face
            //and if we have, return the new face we are on, and the new direction we are facing in

            switch (face)
            {
                case Face.Top:
                    if (position.c < 0)
                    {
                        return (Face.Top, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Top, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Bottom, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Front, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Left:
                    if (position.c < 0)
                    {
                        return (Face.Back, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Front, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Left, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Left, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Back:
                    if (position.c < 0)
                    {
                        return (Face.Front, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Left, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Back, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Back, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                case Face.Front:
                    if (position.c < 0)
                    {
                        return (Face.Left, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Back, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Top, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Bottom, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
                //bottom
                default:
                    if (position.c < 0)
                    {
                        return (Face.Right, (position.r, CUBESIZE - 1), (0, -1));
                    }
                    if (position.c >= CUBESIZE)
                    {
                        return (Face.Right, (position.r, 0), (0, 1));
                    }
                    if (position.r < 0)
                    {
                        return (Face.Front, (CUBESIZE - 1, position.c), (-1, 0));
                    }
                    if (position.r >= CUBESIZE)
                    {
                        return (Face.Top, (0, position.c), (1, 0));
                    }
                    return (face, position, direction);
            }
        }
    }
}