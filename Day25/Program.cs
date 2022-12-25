using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Day25
{
    internal class Program
    {

        static (int, string)[] tests = new[]
        {
            (1,"1"),
            (2,"2"),
            (3,"1="),
            (4,"1-"),
            (5,"10"),
            (6,"11"),
            (7,"12"),
            (8,"2="),
            (9,"2-"),
            (10,"20"),
            (15,"1=0"),
            (20,"1-0"),
            (2022,"1=11-2"),
            (12345,"1-0---0"),
            (314159265,"1121-1110-1=0"),
        };

        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach ((int dec, string elf) in tests)
            {
                var converted = convertToElf(dec);
                if (converted != elf)
                {
                    throw new Exception($"Boom: {dec} was converted to {converted} instead of {elf}");
                }
            }

            var elfNumbers = File.ReadAllLines($"{args[0]}.txt");
            var decimalNumbers = new List<long>();
            foreach (var elf in elfNumbers)
            {
                long units = 1;
                long number = 0;
                foreach (var ch in elf.Reverse())
                {
                    number += units * ch switch
                    {
                        '=' => -2,
                        '-' => -1,
                        '0' => 0,
                        '1' => 1,
                        '2' => 2,
                    };
                    units *= 5;
                }

                decimalNumbers.Add(number);
            }

            var sum = decimalNumbers.Sum();



            var part1 = convertToElf(sum);
            Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}ms");
        }

        private static string convertToElf(long number)
        {
            StringBuilder sb = new StringBuilder();
            {
                long units = 1;
                while (true)
                {
                    var rem = number % (units * 5) / units;
                    var div = number / (units * 5);
                    sb.Append(rem.ToString());
                    if (div == 0)
                    {
                        break;
                    }
                    units *= 5;
                }
            }

            var sumDigits = sb.ToString().Select(ch => int.Parse(ch.ToString()));

            int carry = 0;
            List<int> digits = new List<int>();
            foreach (var digit in sumDigits)
            {
                var newDigit = digit + carry;
                carry = 0;

                if (newDigit >= 3)
                {
                    carry = 1;
                    newDigit -= 5;
                }
                digits.Add(newDigit);
            }
            if (carry == 1)
            {
                digits.Add(1);
            }

            digits.Reverse();
            StringBuilder answer = new StringBuilder();
            foreach (var digit in digits)
            {
                answer.Append(digit switch
                {
                    -2 => '=',
                    -1 => '-',
                    0 => '0',
                    1 => '1',
                    2 => '2',
                    _ => throw new Exception($"BOOM!"),
                });
            }

            return answer.ToString();
        }

    }
}
