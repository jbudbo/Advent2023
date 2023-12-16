using cs;
using System.Diagnostics.CodeAnalysis;

internal sealed class Day16 : AdventBase<char[]>
{
    protected override string Day => nameof(Day16);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
        => row.Data.Valid ? Union.Pass(row.Data.Value!.ToArray()) : Union.Fail<char[]>();

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        List<char[]> map = [];
        await foreach (var (_, (_, rowData)) in source)
        {
            map.Add(rowData);
        }

        Answer = ChargesFromOrigin([.. map], (0,0, Direction.East));
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        List<char[]> map = [];
        await foreach (var (_, (_, rowData)) in source)
        {
            map.Add(rowData);
        }

        char[][] plane = [.. map];

        int sY = plane.Length - 1, eX = plane[0].Length - 1;

        for (int i = 0; i < sY; i++)
        {
            long a = ChargesFromOrigin(plane, (0, i, Direction.East));
            Answer = Math.Max(Answer, a);

            long b = ChargesFromOrigin(plane, (eX, i, Direction.West));
            Answer = Math.Max(Answer, b);
        }

        for (int i = 0; i < eX; i++)
        {
            Answer = Math.Max(Answer, ChargesFromOrigin(plane, (i, 0, Direction.South)));
            Answer = Math.Max(Answer, ChargesFromOrigin(plane, (i, eX, Direction.North)));
        }
    }

    [DebuggerStepThrough]
    private static long ChargesFromOrigin(in char[][] map, (int,int,Direction) originPoint)
    {
        Queue<(int, int, Direction)> work = [];
        work.Enqueue(originPoint);

        int xMax = map[0].Length;
        int yMax = map.Length;

        HashSet<(int, int, Direction)> alreadyTaken = new(new StepComparer());
        HashSet<(int, int)> charges = new(new CoordComparer());

        while (work.TryDequeue(out var origin))
        {
            var (x, y, d) = origin;

            if (x < 0 || x >= xMax || y < 0 || y >= yMax)
                continue;

            //  We've already traversed this path, no need to go again
            if (alreadyTaken.Contains(origin)) continue;

            alreadyTaken.Add(origin);
            charges.Add((x, y));


            char point = map[y][x];
            switch (point)
            {
                case '.':
                case '|' when d is Direction.North or Direction.South:
                case '-' when d is Direction.East or Direction.West:
                    work.Enqueue(Step(origin));
                    break;
                case '|' when d is Direction.East or Direction.West:
                    work.Enqueue(RotCC(origin));
                    work.Enqueue(RotC(origin));
                    break;
                case '-' when d is Direction.North or Direction.South:
                    work.Enqueue(RotCC(origin));
                    work.Enqueue(RotC(origin));
                    break;
                case '/' when d is Direction.East or Direction.West:
                    work.Enqueue(RotCC(origin));
                    break;
                case '/' when d is Direction.North or Direction.South:
                    work.Enqueue(RotC(origin));
                    break;
                case '\\' when d is Direction.East or Direction.West:
                    work.Enqueue(RotC(origin));
                    break;
                case '\\' when d is Direction.North or Direction.South:
                    work.Enqueue(RotCC(origin));
                    break;
                default:
                    throw new UnreachableException("I can't let you do that Dave");
            }
        }

        return charges.Count;
    }

    [DebuggerStepThrough]
    private static (int, int, Direction) RotCC((int x, int y, Direction direction)t) => t.direction switch
    {
        Direction.North => (t.x - 1, t.y, Direction.West),
        Direction.East => (t.x, t.y - 1, Direction.North),
        Direction.South => (t.x + 1, t.y, Direction.East),
        Direction.West => (t.x, t.y + 1, Direction.South),
        _ => throw new UnreachableException("I can't let you do that Dave")
    };

    [DebuggerStepThrough]
    private static (int, int, Direction) RotC((int x, int y, Direction direction)t) => t.direction switch
    {
        Direction.North => (t.x + 1, t.y, Direction.East),
        Direction.East => (t.x, t.y + 1, Direction.South),
        Direction.South => (t.x - 1, t.y, Direction.West),
        Direction.West => (t.x, t.y - 1, Direction.North),
        _ => throw new UnreachableException("I can't let you do that Dave")
    };

    [DebuggerStepThrough]
    private static (int, int, Direction) Step((int x, int y, Direction direction)t) => t.direction switch
    {
        Direction.North => (t.x, t.y - 1, t.direction),
        Direction.East => (t.x + 1, t.y, t.direction),
        Direction.South => (t.x, t.y + 1, t.direction),
        Direction.West => (t.x - 1, t.y, t.direction),
        _ => throw new UnreachableException("I can't let you do that Dave")
    };

    [DebuggerStepThrough]
    private sealed class CoordComparer : IEqualityComparer<(int, int)>
    {
        public bool Equals((int, int) x, (int, int) y)
            => x.Item1 == y.Item1
            && x.Item2 == y.Item2;

        public int GetHashCode([DisallowNull] (int, int) obj)
            => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
    }

    [DebuggerStepThrough]
    private sealed class StepComparer : IEqualityComparer<(int, int, Direction)>
    {
        public bool Equals((int, int, Direction) x, (int, int, Direction) y)
            => x.Item1 == y.Item1
            && x.Item2 == y.Item2
            && x.Item3 == y.Item3;

        public int GetHashCode([DisallowNull] (int, int, Direction) obj)
            => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode() ^ obj.Item3.GetHashCode();
    }

    enum Direction : byte
    {
        North, East, South, West
    }
}
