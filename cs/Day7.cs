using System.Runtime.CompilerServices;

namespace cs;

internal static partial class Day7
{
    internal static async ValueTask<long> P2(Stream input, CancellationToken cancellationToken = default)
    {
        long answer = 0;
        List<Hand> hands = [];

        using StreamReader reader = new(input);
        await foreach (var hand in ReadInput(reader, cancellationToken))
        {
            hands.Add(hand);
        }

        Hand[] sortedHands = [.. hands.Order(JokerHandComparer.Instance)];

        for (int i = 1, j = sortedHands.Length; i <= j; i++)
        {
            Hand h = sortedHands[i - 1];
            string cards = string.Create(5, h.Cards, (chars, cards) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    chars[i] = cards[i];
                }
            });
            Console.WriteLine($"{h.Bid:000}: {cards} ({h.JType}) :{h.Bid * i}");
            answer += h.Bid * i;
        }

        return answer;
    }

    internal static async ValueTask<long> P1(Stream input, CancellationToken cancellationToken = default)
    {
        long answer = 0;
        List<Hand> hands = [];

        using StreamReader reader = new(input);
        await foreach(var hand in ReadInput(reader, cancellationToken))
        {
            hands.Add(hand);
        }

        Hand[] sortedHands = [.. hands.Order(HandComparer.Instance)];

        for (int i = 1, j =  sortedHands.Length; i <= j; i++)
        {
            answer += sortedHands[i-1].Bid * i;
        }

        return answer;
    }

    private static async IAsyncEnumerable<Hand> ReadInput(StreamReader rdr
        , [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!rdr.EndOfStream)
        {
            string line = await rdr.ReadLineAsync(cancellationToken);

            yield return new Hand(line);
        }
    }
}

file sealed class JokerHandComparer : IComparer<Hand>
{
    public static readonly JokerHandComparer Instance = new();

    public int Compare(Hand x, Hand y)
    {
        int diff = x.JType - y.JType;

        if (diff is not 0)
            return diff;

        //If they're the same value, we need to check the cards
        int i = 0;
        while (i < 5)
        {
            diff = x.Cards[i].JokerValue - y.Cards[i].JokerValue;
            if (diff is not 0)
                return diff;
            i++;
        }

        return 0;
    }
}


file sealed class HandComparer : IComparer<Hand>
{
    public static readonly HandComparer Instance = new();

    public int Compare(Hand x, Hand y)
    {
        int diff = x.Type - y.Type;

        if (diff is not 0)
            return diff;

        //If they're the same value, we need to check the cards
        int i = 0;
        while (i < 5)
        {
            diff = x.Cards[i] - y.Cards[i];
            if (diff is not 0)
                return diff;
            i++;
        }

        return 0;
    }
}

internal readonly struct Hand : IComparable<Hand>
{
    public readonly int Bid { get; }

    public readonly Card[] Cards { get; }

    public Hand(string data)
    {
        string[] details = data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Cards = details[0].Select(c => new Card(c)).ToArray();

        Bid = int.Parse(details[1]);
    }

    public readonly HandType Type => DetermineHandType();
    public readonly HandType JType => DetermineJokerHandType();

    private HandType DetermineHandType()
    {
        ReadOnlySpan<(int, int)> groups = Cards
            .GroupBy(static c => c.Value)
            .Select(static g => (g.Key, g.Count()))
            .OrderByDescending(static g => g.Item2)
            .ToArray();

        return groups[0].Item2 switch
        {
            1 => HandType.HighCard,
            2 when groups[1].Item2 is 1 => HandType.OnePair,
            2 when groups[1].Item2 is 2 => HandType.TwoPair,
            3 when groups.Length >= 2 && groups[1].Item2 is 2 => HandType.FullHouse,
            3 when groups.Length >= 3 && groups[2].Item2 is 1 => HandType.ThreeOfAKind,
            4 => HandType.FourOfAKind,
            5 => HandType.FiveOfAKind,
            _ => HandType.Err
        };
    }

    private HandType DetermineJokerHandType()
    {
        int jokerCount = Cards.Count(static c => c.JokerValue is 0);

        (int, int)[] groups = Cards
            .Where(static c => c.JokerValue is not 0)
            .GroupBy(static c => c.JokerValue)
            .Select(static g => (g.Key, g.Count()))
            .OrderByDescending(static g => g.Item2)
            .ToArray();

        if (groups.Length is 0)//All Jokers
            return HandType.FiveOfAKind;

        //  Add any jokers to whatever is already the highest
        //      if anything
        groups[0].Item2 += jokerCount;

        return groups[0].Item2 switch
        {
            1 => HandType.HighCard,
            2 when groups[1].Item2 is 1 => HandType.OnePair,
            2 when groups[1].Item2 is 2 => HandType.TwoPair,
            3 when groups.Length >= 2 && groups[1].Item2 is 2 => HandType.FullHouse,
            3 when groups.Length >= 3 && groups[2].Item2 is 1 => HandType.ThreeOfAKind,
            4 => HandType.FourOfAKind,
            5 => HandType.FiveOfAKind,
            _ => HandType.Err
        };
    }

    public override string ToString()
        => $"{Bid}: {Cards[0]}-{Cards[1]}-{Cards[2]}-{Cards[3]}-{Cards[4]} :{Type}";

    public int CompareTo(Hand other)
    {
        int diff = Type - other.Type;

        if (diff is not 0)
            return diff;

        //If they're the same value, we need to check the cards
        int i = 0;
        while (i < 5)
        {
            diff = Cards[i] - other.Cards[i];
            if (diff is not 0)
                return diff;
            i++;
        }

        return 0;
    }
}

internal enum HandType : int
{
    Err = 0,
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    FullHouse = 5,
    FourOfAKind = 6,
    FiveOfAKind = 7
}

internal readonly struct Card(char card) : IComparable<Card>
{
    private static readonly char[] CARDS = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];
    private static readonly char[] JCARDS = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];

    public readonly char Face => card;
    public readonly int Value => Array.IndexOf(CARDS, card);
    public readonly int JokerValue => Array.IndexOf(JCARDS, card);

    public static implicit operator char(Card card) => card.Face;
    public static implicit operator Card(char c) => new(c);
    public static implicit operator int(Card c) => c.Value;

    public override readonly string ToString()
        => $"{Value} '{Face}'";

    public readonly int CompareTo(Card other)
        => Value - other.Value;
}