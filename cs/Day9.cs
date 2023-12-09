namespace cs;

internal sealed class Day9 : AdventBase<AdventSequence>
{
    protected override string Day => nameof(Day9);

    protected override DeterminateOf<AdventSequence> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (v, d)) = row;

        if (!v) return Union.Fail<AdventSequence>();

        string[] points = SplitString(d!);
        long[] dataEntries = points.Select(long.Parse).ToArray();

        return Union.Pass(new AdventSequence(dataEntries));
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<AdventSequence>> source, CancellationToken cancellationToken = default)
    {
        await foreach(DataLine<AdventSequence> s in source)
        {
            var (_, (v, d)) = s;

            if (!v) continue;

            // Determines the head value recursively
            //  kind of nasty but it'll tackle part 1
            Answer += d.GetNext();

        }
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<AdventSequence>> source, CancellationToken cancellationToken = default)
    {
        await foreach (DataLine<AdventSequence> s in source)
        {
            var (_, (v, d)) = s;

            if (!v) continue;

            // Determines the head value recursively
            //  kind of nasty but it'll tackle part 1
            long p = d.GetPrevious();
            Answer += p;
        }
    }
}

[DebuggerDisplay("{DebugView}")]
internal readonly struct AdventSequence(long[] data)
{
    public readonly DeterminateOf<AdventSequence> Delta => IsTail
        ? Union.Fail<AdventSequence>()
        : Union.Pass<AdventSequence>(new(data
            .Window()
            .Select(static w => w.Item2 - w.Item1)
            .ToArray()));

    public readonly bool IsUniform => (data.Sum() / data.Length) == data[0];

    public readonly bool IsTail => IsUniform && data[0] is 0;

    public readonly long GetNext()
    {
        if (IsTail) return 0;

        //  Shouldn't ever happen buuuuut
        if (!Delta.Valid) return 0;

        long dNext = Delta.Value.GetNext();

        return data[^1] + dNext;
    }

    public readonly long GetPrevious()
    {
        if (IsTail) return 0;

        //  Shouldn't ever happen buuuuut
        if (!Delta.Valid) return 0;

        long dNext = Delta.Value.GetPrevious();

        return data[0] - dNext;
    }

    private string DebugView => string.Join(' ', data);
}