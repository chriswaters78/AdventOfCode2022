using System.Text;

namespace Day25
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

            var part1 = convertToElf(decimalNumbers.Sum());
            Console.WriteLine($"Part 1: {part1}");
        }

        private static string convertToElf(long number)
        {
            var sb = new StringBuilder();
            long units = 5;
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

            bool carry = false;
            var digits = new List<int>();
            foreach (var digit in sb.ToString().Select(ch => int.Parse(ch.ToString())))
            {
                var newDigit = digit + (carry ? 1 : 0);
                carry = false;

                if (newDigit >= 3)
                {
                    carry = true;
                    newDigit -= 5;
                }
                digits.Add(newDigit);
            }
            if (carry)
            {
                digits.Add(1);
            }

            digits.Reverse();
            var answer = new StringBuilder();
            foreach (var digit in digits)
            {
                answer.Append(digit switch
                {
                    -2 => '=',
                    -1 => '-',
                    0 => '0',
                    1 => '1',
                    2 => '2',
                });
            }

            return answer.ToString();
        }
    }
}
