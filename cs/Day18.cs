
internal sealed partial class Day18 : AdventBase<Day18.Instruction>
{
    protected override string Day => nameof(Day18);

    internal readonly struct Instruction
    {
        public Heading Heading { get; init; }
        public int Steps { get; init; } 
        public string HexColor { get; init; }
    }

    internal enum Heading : byte
    {
        North, East, South, West
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<Instruction>> source, CancellationToken cancellationToken = default)
    {
        int x = 0, y = 0;

        long distance = 0;
        long vx = 0, vy = 0;
        await foreach(var row in source)
        {
            var (_, (v, data)) = row;

            if (v)
            {
                int px = x;
                int py = y;

                distance += data.Steps;

                switch (data.Heading)
                {
                    case Heading.North:
                        y -= data.Steps;
                        break;
                    case Heading.East:
                        x += data.Steps;
                        break;
                    case Heading.South:
                        y += data.Steps;
                        break;
                    case Heading.West:
                        x -= data.Steps;
                        break;
                }

                vx += px * y;
                vy += py * x;
            }
        }

        var z = Math.Abs(vx - vy) + distance;
        
        Answer = z / 2 + 1;
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<Instruction>> source, CancellationToken cancellationToken = default)
    {
        long x = 0, y = 0;

        long distance = 0;
        long vx = 0, vy = 0;
        await foreach (var row in source)
        {
            var (_, (v, data)) = row;

            if (v)
            {
                var (h, s) = Decode(data.HexColor.AsSpan()[1..]);

                long px = x;
                long py = y;

                distance += s;

                switch (h)
                {
                    case Heading.North:
                        y -= s;
                        break;
                    case Heading.East:
                        x += s;
                        break;
                    case Heading.South:
                        y += s;
                        break;
                    case Heading.West:
                        x -= s;
                        break;
                }

                vx += px * y;
                vy += py * x;
            }
        }

        var z = Math.Abs(vx - vy) + distance;

        Answer = z / 2 + 1;
    }

    private static (Heading, long) Decode(ReadOnlySpan<char> source)
    {
        Heading h = (source[^1] - '0') switch
        {
            0 => Heading.East,
            1 => Heading.South,
            2 => Heading.West,
            3 => Heading.North,
            _ => throw new UnreachableException()
        };

        Span<byte> lbuff = stackalloc byte[sizeof(long)];
        ReadOnlySpan<byte> buffer = Convert.FromHexString($"0{source[..^1]}");
        for (int i = 0, j = buffer.Length; i < j; i++)
        {
            lbuff[i] = buffer[^(i+1)];
        }

        return (h, BitConverter.ToInt64(lbuff));
    }

    protected override Uof<Instruction> ParseData(DataLine<string?> row, bool PartTwo)
    {
        if (!row.Data.Valid) return Union.Fail<Instruction>();

        string[] parts = SplitString(row.Data.Value!, 3);

        Heading h = parts[0] switch
        {
            "R" => Heading.East,
            "D" => Heading.South,
            "L" => Heading.West,
            "U" => Heading.North,
            _ => throw new UnreachableException()
        };
        
        return Union.Pass(new Instruction { Heading = h, Steps = int.Parse(parts[1]), HexColor = parts[2][1..^1] });
    }
}