namespace Day2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt").Select(str => (str[0], str[2])).ToList();

            int totalScore1 = 0;
            int totalScore2 = 0;
            foreach (var tp in lines)
            {
                var result = win(tp) + choice(tp.Item2);
                totalScore1 += result;

                totalScore2 += win2(tp);
            }

            Console.WriteLine(totalScore1);
            Console.WriteLine(totalScore2);
        }
        static int choice(char a)
        {
            return a - 'X' + 1;
        }

        static int win((char a, char b) tp)
        {
            switch (tp)
            {
                case ('A', 'X'):
                case ('B', 'Y'):
                case ('C', 'Z'):
                    return 3;
                case ('A', 'Z'):
                case ('B', 'X'):
                case ('C', 'Y'):
                    return 0;
                default:
                    return 6;
            }
        }
        static int win2((char a, char b) tp)
        {
            switch (tp)
            {
                //rock / loss
                case ('A', 'X'):
                    return 3;
                //rock / draw
                case ('A', 'Y'):
                    return 4;
                //rock / win
                case ('A', 'Z'):
                    return 8;
                
                //paper / loss
                case ('B', 'X'):
                    return 1;
                //paper / draw
                case ('B', 'Y'):
                    return 5;
                //paper / win
                case ('B', 'Z'):
                    return 9;
                
                //scissors / loss
                case ('C', 'X'):
                    return 2;
                //scissors / draw
                case ('C', 'Y'):
                    return 6;
                //scissors / win
                case ('C', 'Z'):
                    return 7;
                default:
                    throw new Exception();
            }
        }
    }
}