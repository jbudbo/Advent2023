using Point = (int x, int y);

namespace cs;

internal static partial class Day3
{
    public const string P1_EXAMPLE = """
    467..114..
    ...*......
    ..35..633.
    ......#...
    617*......
    .....+.58.
    ..592.....
    ......755.
    ...$.*....
    .664.598..
    """;
    public const int P1_ANSWER = 4361;

    public static int P2(string data)
    {
        int answer = 0;

        Dictionary<int, Dictionary<int, Range>> numbers = [];
        Dictionary<Point, char> symbols = [];

        ParseInput(data, numbers, symbols);

        HashSet<(Range,Range)> qualifiers = [];

        FindGears(numbers, symbols, qualifiers);

        foreach (var (l,r) in qualifiers)
        {
            answer += int.Parse(data[l]) * int.Parse(data[r]);
        }

        Console.WriteLine();
        return answer;
    }

    public static int P1(string data)
    {
        int answer = 0;

        Dictionary<int, Dictionary<int,Range>> numbers = [];
        Dictionary<Point, char> symbols = [];

        ParseInput(data, numbers, symbols);

        HashSet<Range> qualifiers = [];

        FindQualifiers(numbers, symbols, qualifiers);

        string[] lines = data.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Console.CursorVisible = false;
        Console.WindowWidth = lines[0].Length;
        Console.WindowHeight = lines.Length;
        for (int y = 0, yl = lines.Length; y < yl; y++)
        {
            for (int x = 0, xl = lines[y].Length; x < xl; x++)
            {
                if (symbols.ContainsKey((x, y)))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(lines[y][x]);
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }

                if (numbers.TryGetValue(y, out var d) && d.TryGetValue(x, out var c))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.CursorLeft -= c.End.Value - c.Start.Value - 1;
                    Console.Write(data[c]);
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                Console.Write(lines[y][x]);
            }
            Console.WriteLine();
        }

        foreach(Range r in qualifiers)
        {
            answer += int.Parse(data[r]);
        }

        Console.WriteLine();
        return answer;
    }

    private static void FindGears(Dictionary<int, Dictionary<int, Range>> numbers, IReadOnlyDictionary<Point, char> symbols, HashSet<(Range,Range)> qualifiers)
    {
        Span<int> symbolCover = stackalloc int[3];
        
        foreach (var ((x, y), c) in symbols)
        {
            if (c is not '*') continue;

            Range a = default, b = default;
            bool leftFound = false;
            for (int i = y - 1; i <= y + 1; i++)
            {
                if (!numbers.ContainsKey(i)) continue;

                foreach (var (cx, cr) in numbers[i])
                {
                    ReadOnlySpan<int> numberCover = cr.Spread(cx);
                    symbolCover[0] = x - 1;
                    symbolCover[1] = x;
                    symbolCover[2] = x + 1;
                    if (symbolCover.ContainsAny(numberCover))
                    {
                        if (!leftFound)
                        {
                            a = cr;
                            leftFound = true;
                            continue;
                        }
                        if (leftFound)
                        {
                            b = cr;
                            qualifiers.Add((a, b));
                            leftFound = false;
                            break;
                        }
                    }
                }
            }
        }
    }

    private static void FindQualifiers(Dictionary<int, Dictionary<int, Range>> numbers, IReadOnlyDictionary<Point, char> symbols, HashSet<Range> qualifiers)
    {
        Span<int> symbolCover = stackalloc int[3];
        foreach (var ((x, y), c) in symbols)
        {
            for (int i = y - 1; i <= y + 1; i++)
            {
                if (!numbers.ContainsKey(i)) continue;

                foreach (var (cx, cr) in numbers[i])
                {
                    ReadOnlySpan<int> numberCover = cr.Spread(cx);
                    symbolCover[0] = x - 1;
                    symbolCover[1] = x;
                    symbolCover[2] = x + 1;
                    if (symbolCover.ContainsAny(numberCover))
                    {
                        qualifiers.Add(cr);
                    }
                }
            }
        }
    }

    private static void ParseInput(in ReadOnlySpan<char> data, Dictionary<int, Dictionary<int, Range>> numbers, Dictionary<Point, char> symbols)
    {
        bool onNewLineWatch = false, observingNumber = false;
        int l = data.Length;//Our max
        int i = 0; //Where we are through the input
        int x = 0, y = 0; //Grid Origin
        int s = -1; //The start of a range;

        while (i < l)
        {
            char c = data[i];
            switch (c)
            {
                case '.':
                    if (observingNumber)
                    {
                        numbers.GetOrAdd(y, [])[x-1] = new(s, i);
                    }
                    observingNumber = false;
                    break;

                case '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or '0':
                    if (!observingNumber)
                    {
                        s = i;
                    }
                    observingNumber = true;
                    break;

                case '\r' or '\n':
                    if (observingNumber)
                    {
                        numbers.GetOrAdd(y, [])[x - 1] = new(s, i);
                    }
                    observingNumber = false;
                    if (!onNewLineWatch)
                        onNewLineWatch = true;
                    else
                    {
                        onNewLineWatch = false;
                        x = -1;
                        y++;
                    }
                    break;

                default: //Any other symbol
                    if (observingNumber)
                    {
                        numbers.GetOrAdd(y, [])[x-1] = new(s, i);
                    }
                    symbols[(x, y)] = c;
                    observingNumber = false;
                    break;
            }
            x++;
            i++;
        }
    }
}

file static class Extensions
{
    internal static int[] Spread(this Range r, int origin)
    {
        int[] buffer = new int[r.End.Value - r.Start.Value];

        for (int i = 0, l = buffer.Length; i < l; i++)
        {
            buffer[i] = origin - i;
        }

        Array.Sort(buffer);

        return buffer;
    }

    internal static V GetOrAdd<K,V>(this IDictionary<K,V> d, K key, V addValue = default)
    {
        if (!d.ContainsKey(key))
        {
            d[key] = addValue;
        }
        return d[key];
    }
}