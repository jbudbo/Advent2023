using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace cs;

internal static partial class Day1
{
    public const string P1_EXAMPLE = """
        1abc2
        pqr3stu8vwx
        a1b2c3d4e5f
        treb7uchet
        """;

    public const string P2_EXAMPLE = """
        two1nine
        eightwothree
        abcone2threexyz
        xtwone3four
        4nineeightseven2
        zoneight234
        7pqrstsixteen
        """;

    public const int P1_ANSWER = 142;
    public const int P2_ANSWER = 281;

    public static int Calibrate(string set)
    {
        ReadOnlySpan<string> lines = set.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        ref string line = ref MemoryMarshal.GetReference(lines);
        ref string lastLine = ref Unsafe.Add(ref line, lines.Length);

        int sum = 0;
        while (Unsafe.IsAddressLessThan(ref line, ref lastLine))
        {
            ReadOnlySpan<char> chars = line;
            int value = 0;
            byte found = 0;
            for (int i = 0, j = chars.Length; i < j;i++)
            {
                if (value < 10 && char.IsDigit(chars[i]))
                {
                    value += 10 * (chars[i] - 48);
                    found++;
                }

                //  Our input data does not appear to carry any zeros
                if (value % 10 is 0 && char.IsDigit(chars[^(i+1)]))
                {
                    value += chars[^(i+1)] - 48;
                    found++;
                }

                if (found is 2)
                    break;
            }
            if (found is 1)
            {
                value += value / 10;
            }
            sum += value;
            line = ref Unsafe.Add(ref line, 1);
        }

        return sum;
    }

    public static int ReCalibrate(string set)
    {
        ReadOnlySpan<string> lines = set.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        ref string line = ref MemoryMarshal.GetReference(lines);
        ref string lastLine = ref Unsafe.Add(ref line, lines.Length);

        Regex r = GetNumIdentifyRegex();
        int sum = 0;
        while (Unsafe.IsAddressLessThan(ref line, ref lastLine))
        {
            ReadOnlySpan<char> chars = line;
            int value = 0;
            byte found = 0;
            void AddNumber(in int v)
            {
                if (found is 0)
                {
                    value += v * 10;
                    found++;
                }
                else
                {
                    value = value - (value % 10) + v;
                    found++;
                }
            }
            foreach (ValueMatch m in r.EnumerateMatches(chars))
            {
                if (char.IsDigit(chars[m.Index]))
                    AddNumber(chars[m.Index] - 48);
                else
                {
                    switch (char.ToLower(chars[m.Index]))
                    {
                        case 'o':
                            AddNumber(1);
                            break;
                        case 'e':
                            AddNumber(8);
                            break;
                        case 'n':
                            AddNumber(9);
                            break;
                        case 't':
                            switch (char.ToLower(chars[m.Index + 1]))
                            {
                                case 'w':
                                    AddNumber(2);
                                    break;
                                case 'h':
                                    AddNumber(3);
                                    break;
                                default: throw new UnreachableException();
                            }
                            break;
                        case 'f':
                            switch (char.ToLower(chars[m.Index + 1]))
                            {
                                case 'o':
                                    AddNumber(4);
                                    break;
                                case 'i':
                                    AddNumber(5);
                                    break;
                                default: throw new UnreachableException();
                            }
                            break;
                        case 's':
                            switch (char.ToLower(chars[m.Index + 1]))
                            {
                                case 'i':
                                    AddNumber(6);
                                    break;
                                case 'e':
                                    AddNumber(7);
                                    break;
                                default: throw new UnreachableException();
                            }
                            break;
                        default: throw new UnreachableException();
                    }
                }
            }

            if (found is 1)
            {
                // We never found a second number...
                AddNumber(value / 10);
            }
            Debug.WriteLine($"{line} -> {value}");
            sum += value;
            line = ref Unsafe.Add(ref line, 1);
        }

        return sum;
    }

    [GeneratedRegex("(?=(\\d|one|two|three|four|five|six|seven|eight|nine))", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex GetNumIdentifyRegex();
}