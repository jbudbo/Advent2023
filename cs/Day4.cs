using System.Collections.Immutable;
using System.Formats.Asn1;

namespace cs;

internal static partial class Day4
{
    public const string P1_EXAMPLE = """
    Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
    Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
    Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
    Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
    Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
    Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
    """;
    public const int P1_ANSWER = 13;

    internal static int P2(string input)
    {
        Dictionary<int, (HashSet<Range>, HashSet<Range>)> resultSet = [];

        InterpretInput(input, resultSet);

        Dictionary<int, int> cardWinnings = new(resultSet.Count);
        foreach(var (c, (w, m))in resultSet)
        {
            ImmutableHashSet<int> winners = w
                .Select(r => int.Parse(input[r]))
                .ToImmutableHashSet();

            int winningCount = m
                .Select(r => int.Parse(input[r]))
                .Where(winners.Contains)
                .Count();

            cardWinnings[c] = winningCount;
        }

        PriorityQueue<int, int> q = new();
        foreach (var (k, v) in cardWinnings)
            q.Enqueue(v, k);

        int answer = 0;
        while(q.TryDequeue(out int v, out int k))
        {
            answer++;
            for (int i = 1; i <= v; i++)
            {
                q.Enqueue(cardWinnings[k + i], k + i);
            }
        }

        return answer;
    }

    internal static int P1(string input)
    {
        int answer = 0;

        Dictionary<int, (HashSet<Range>, HashSet<Range>)> resultSet = [];

        InterpretInput(input, resultSet);

        foreach(var play in resultSet)
        {
            int cardValue = OutputPlay(input, play);

            answer += cardValue;
        }

        return answer;
    }

    private static int OutputPlay(string data, KeyValuePair<int, (HashSet<Range>, HashSet<Range>)> play)
    {
        ConsoleColor original = Console.ForegroundColor;
        var (card, (winners, my)) = play;

        Console.Write("Card ");

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(data[card]);
        Console.ForegroundColor = original;

        Console.Write(": ");

        Console.Write(string.Join(' ', winners.Select(r => data[r])));

        Console.Write(" |");

        int value = 0;
        
        ImmutableHashSet<int> w = winners
            .Select(r => int.Parse(data[r]))
            .ToImmutableHashSet();

        foreach (int n in my.Select(r => int.Parse(data[r])))
        {
            Console.Write(" ");
            if (w.Contains(n))
            {
                if (value is 0)
                    value = 1;
                else
                    value *= 2;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(n);
                Console.ForegroundColor = original;
            }
            else
            {
                Console.Write(n);
            }
        }

        Console.Write(" = ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(value);
        
        Console.ForegroundColor = original;
        return value;
    }

    private static void InterpretInput(ReadOnlySpan<char> data, IDictionary<int, (HashSet<Range>, HashSet<Range>)> cards)
    {
        int place = 0;
        while (place < data.Length)
        {
            Range cardNumber = FindCardNumber(data, ref place);
            HashSet<Range> winners = ReadWinners(data, ref place);
            HashSet<Range> numbers = ReadPlayNumbers(data, ref place);

            int c = int.Parse(data[cardNumber]);
            cards[c] = (winners,numbers);
        }
    }

    private static HashSet<Range> ReadPlayNumbers(in ReadOnlySpan<char> data, ref int position)
    {
        HashSet<Range> numbers = [];
        int start = -1;
        char c = data[position];
        while (c is not '\r')
        {
            if (c is ' ')
            {
                if (start >= 0)
                {
                    numbers.Add(new(start, position));
                    start = -1;
                }
                c = data[++position];
                continue;
            }

            if (c is not ('0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'))
                //  Really just advancing unless we found something unexpected
                throw new Exception();

            if (start is -1)
                start = position;

            if (++position >= data.Length) break;

            c = data[position];
        }
        if (start >= 0)
        {
            numbers.Add(new(start, position));
        }
        position += 2;
        return numbers;
    }

    private static HashSet<Range> ReadWinners(in ReadOnlySpan<char> data, ref int position)
    {
        HashSet<Range> winners = [];
        int start = -1;
        char c = data[position];
        while (c is not '|')
        {
            if (c is ' ')
            {
                if (start >= 0)
                { 
                    winners.Add(new(start, position));
                    start = -1;
                }
                c = data[++position];
                continue;
            }

            if (c is not ('0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'))
                //  Really just advancing unless we found something unexpected
                throw new Exception();

            if (start is -1)
                start = position;

            c = data[++position];
        }
        position += 2;
        return winners;
    }

    private static Range FindCardNumber(in ReadOnlySpan<char> data, ref int position)
    {
        char c = data[position];
        if (c is not 'C')
            return Range.All;

        //  Expect to skip C a r d and find a number in the next place
        position += 4;

        //  Skip any space padding
        for (; ++position < data.Length && (c = data[position]) is ' ';)
        {}

        int cardStart = position;
        do
        {
            if (c is not ('0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9'))
                //  Really just advancing unless we found something unexpected
                throw new Exception();

            c = data[++position];
        } while (c is not ':');

        Range r = new(cardStart, position);
        position += 2;
        return r;
    }
}