using System.Diagnostics.CodeAnalysis;
using Position = (int X, int Y);
internal sealed partial class Day17 : AdventBase<byte[]>
{
    protected override string Day => nameof(Day17);

    protected override Uof<byte[]> ParseData(DataLine<string?> row, bool PartTwo)
    {
        if (!row.Data.Valid)
            return Union.Fail<byte[]>();

        var prow = row.Data.Value!.Select(static c => (byte)(c - '0'));
        return Union.Pass(prow.ToArray());
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<byte[]>> source, CancellationToken cancellationToken = default)
    {
        byte[][] data = await gather(source, cancellationToken);

        PriorityQueue<(Sim sim, long aggLoss), double> search = new();
        
        int xEnd = data[0].Length, yEnd = data.Length;
        double d = Pythag(xEnd, yEnd);

        search.Enqueue((new(Heading.East, 0, (0,0)),0), d);
        search.Enqueue((new(Heading.South, 0, (0,0)),0), d);

        HashSet<Sim> viewed = [];

        void EnqueueHeading(Sim sim, long aggLoss)
        {
            int valueAt = data[sim.Y][sim.X];
            long newLoss = aggLoss + valueAt;
            double penalty = Pythag(xEnd - sim.X, yEnd - sim.Y);
            search.Enqueue((sim, newLoss), aggLoss);
        }

        long i = 0;
        while (search.TryDequeue(out var step, out _))
        {
            i++;
            //if (i % 1_000 is 0) Debugger.Break();
            //g.DrawChar(step.sim.Position.X, step.sim.Position.Y, step.sim.Heading switch
            //{
            //    Heading.North => "^",
            //    Heading.East => ">",
            //    Heading.West => "<",
            //    Heading.South => "V",
            //    _ => throw new UnreachableException()
            //});

            //  Check if this is an End path
            if (step.sim.X == xEnd - 1 && step.sim.Y == yEnd - 1)
            {
                Answer = step.aggLoss;
                Console.WriteLine($"Answer found in {i} views");
                return;
            }

            if (!step.sim.MustTurn && !viewed.Contains(step.sim.Forward) && step.sim.Forward.Within(xEnd, yEnd))
            {
                //candidates.Add((step.sim, step.aggLoss + data[step.sim.Forward.Y][step.sim.Forward.X]));
                EnqueueHeading(step.sim.Forward, step.aggLoss);
            }

            var (l, r) = step.sim.Turns;

            //  Check if a left turn is even valid
            if (!viewed.Contains(l) && l.Within(xEnd, yEnd))
            {
                //candidates.Add((l, step.aggLoss + data[l.Y][l.X]));
                EnqueueHeading(l, step.aggLoss);
            }

            //  Check if a right turn is even valid
            if (!viewed.Contains(r) && r.Within(xEnd, yEnd))
            {
                //candidates.Add((r, step.aggLoss + data[r.Y][r.X]));
                EnqueueHeading(r, step.aggLoss);
            }

            viewed.Add(step.sim);
        }
    }

    protected override ValueTask PartTwo(IAsyncEnumerable<DataLine<byte[]>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    [DebuggerStepThrough]
    private static double Pythag(int a, int b)
        => Math.Sqrt((a ^ 2) + (b ^ 2));

    private static async Task<byte[][]> gather(IAsyncEnumerable<DataLine<byte[]>> source, CancellationToken cancellationToken = default)
    {
        List<byte[]> data = [];
        await foreach (var (_, (_, rowData)) in source)
            data.Add(rowData);
        return [.. data];
    }

    [DebuggerStepThrough]
    private sealed class CoordComparer : IEqualityComparer<(int, int)>
    {
        public bool Equals((int, int) x, (int, int) y)
            => x.Item1 == y.Item1
            && x.Item2 == y.Item2;

        public int GetHashCode([DisallowNull] (int, int) obj)
            => obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
    }

    [DebuggerDisplay("({X},{Y}) - {Heading}")]
    internal readonly struct Sim : IEqualityComparer<Sim>
    {
        public bool Equals(Sim x, Sim y)
        {
            return x.Heading == y.Heading
                && x.Position.X == y.Position.X
                && x.Position.Y == y.Position.Y;
        }

        public int GetHashCode([DisallowNull] Sim obj)
        {
            return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
        }

        internal Position Position { get; init; }

        internal Heading Heading { get; init; }

        internal uint StepCount { get; init; }

        internal int X => Position.X;

        internal int Y => Position.Y;

        internal Sim(Heading heading, uint stepCount, Position position)
            => (Position, Heading, StepCount)
            = (position, heading, stepCount);

        internal bool MustTurn => StepCount is 3;

        internal Sim Forward => Heading switch
        {
            Heading.North => this with { StepCount = StepCount + 1, Position = (Position.X, Position.Y - 1) },
            Heading.East => this with { StepCount = StepCount + 1, Position = (Position.X + 1, Position.Y) },
            Heading.South => this with { StepCount = StepCount + 1, Position = (Position.X, Position.Y + 1) },
            Heading.West => this with { StepCount = StepCount + 1, Position = (Position.X - 1, Position.Y) },
            _ => throw new UnreachableException()
        };

        internal (Sim Left, Sim Right) Turns => Heading switch
        { 
            Heading.North => (new (Heading.West, 1, (Position.X - 1, Position.Y) ),
                              new (Heading.East, 1, (Position.X + 1, Position.Y) )),
            Heading.South => (new (Heading.East, 1, (Position.X + 1, Position.Y) ),
                              new (Heading.West, 1, (Position.X - 1, Position.Y) )),
            Heading.East => (new (Heading.North, 1, (Position.X, Position.Y - 1) ),
                             new (Heading.South, 1, (Position.X, Position.Y + 1) )),
            Heading.West => (new (Heading.South, 1, (Position.X, Position.Y + 1) ),
                             new (Heading.North, 1, (Position.X, Position.Y - 1) )),
            _ => throw new UnreachableException()

        };

        [DebuggerStepThrough]
        internal bool Within(int x, int y)
            => X >= 0 && X < x && Y >= 0 && Y < y;
    }

    internal enum Heading : byte
    {
        North, East, South, West
    }
}

