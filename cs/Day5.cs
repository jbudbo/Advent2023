namespace cs;

internal sealed class Day5 : AdventBase<RangedMap>
{
    protected override int HeaderLines => 2;
    protected override string Day => nameof(Day5);

    private LongRange[] seeds = [];
    private readonly Dictionary<string, List<RangedMap>> maps = [];
    private string currentMap = string.Empty;

    protected override Uof<RangedMap> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (valid, data)) = row;

        if (!valid)
            return Uof<RangedMap>.Bad();

        // If this is a map definition, prep our dictionary
        if (data!.Contains("map"))
        {
            string mapHeader = SplitString(data!, 2)[0];
            maps[mapHeader] = [];
            currentMap = mapHeader;

            return Uof<RangedMap>.Bad();
        }

        long[] bits = SplitString(data!, 3).Select(long.Parse).ToArray();

        RangedMap map = new(bits);
        return Uof<RangedMap>.Good(map);
    }

    protected override ValueTask PartOne(IAsyncEnumerable<DataLine<RangedMap>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<RangedMap>> source, CancellationToken cancellationToken = default)
    {
        await foreach (var row in source)
        {
            var (_, (valid, data)) = row;
            if (!valid) continue;

            maps[currentMap].Add(data);
        }

        IEnumerable<LongRange> sources = seeds;
        foreach (var k in maps.Keys)
        { 
            sources = DetermineNextSources(sources, maps[k]);
        }

        Answer = sources.OrderBy(static s => s.Start).FirstOrDefault().Start;
    }

    private static IEnumerable<LongRange> DetermineNextSources(IEnumerable<LongRange> sources, List<RangedMap> maps)
    {
        foreach(LongRange source in sources)
        {
            bool fullyContained = false;
            List<RangedMap> overlaps = [];
            foreach(RangedMap map in maps)
            {
                var (s, d) = map;

                //  If this source is completely enveloped by this range map
                //      all we need to is put together the adjusted range of 
                //      destinations
                if (s.Contains(source))
                {
                    fullyContained = true;
                    yield return d + s.Offset(source);
                    break;
                }

                //  If we got here, the source may be completely outside the range or overlap slightly 
                if (s.Overlaps(source))
                    overlaps.Add(map);
            }
            if (fullyContained)
                continue;

            //  If we got over all maps and never found any overlap, we can't map in any way so just
            //      return this source as all sources are in tact
            if (overlaps.Count is 0)
            {
                yield return source;
                continue;
            }

            //  Now we're in trouble, here we found at least one overlap.
            //      For each we need to break up the ranges across what can be mapped and what can't
            //      and how we deal with that
            List<LongRange> outliers = [];
            foreach (RangedMap map in overlaps)
            {
                var (s, d) = map;

                //  Yield back the mapping of the overlay
                var offset = s.Overlay(source).Offset(s);
                yield return d + offset;
                
                //  Yield back anything just prior if applicable
                //      Yields as is because there is no mappable destination
                if (s.Head(source) is { Length: > 0 } hlr)
                    outliers.Add(hlr);

                //  Yield back anything just after if applicable
                //      Yields as is because there is no mappable destination
                if (s.Tail(source) is { Length: > 0 } tlr)
                    outliers.Add(tlr);
            }

            if (outliers.Count is 0) continue;

            foreach (LongRange outlier in DetermineNextSources(outliers, overlaps))
            {
                yield return outlier;
            }
        }
    }

    protected override void ParseHeader(DataLine<string?> header, bool PartTwo)
    {
        var (_, (valid, data)) = header;

        if (!valid) return;

        string seedSet = SplitString(data!, 2)[1];

        if (!PartTwo)
        {
            seeds = SplitString(seedSet)
                .Select(long.Parse)
                .Select(static l => new LongRange(l, 1))
                .ToArray();
        }
        else
        {
            seeds = SplitString(seedSet)
                .Select(long.Parse)
                .Chunk(2)
                .Select(static l => new LongRange(l[0], l[1]))
                .ToArray();
        }
    }
}

[DebuggerStepThrough]
[DebuggerDisplay("{Source} - {Destination}")]
internal readonly struct RangedMap(long[] points)
{
    private readonly LongRange destination = new(points[0], points[2]);
    private readonly LongRange source = new(points[1], points[2]);

    public readonly LongRange Destination => destination;
    public readonly LongRange Source => source;

    public void Deconstruct(out LongRange Source, out LongRange Destination)
        => (Source, Destination)
        = (source, destination);

    internal readonly long GetDestinationFor(long source)
    {
        if (Source.Contains(source))
            return -1;

        long offset = Source.IndexOf(source);

        return destination[offset];
    }
}

[DebuggerStepThrough]
[DebuggerDisplay("{Start}..{End}")]
internal readonly struct LongRange(long start, long length)
{
    public readonly long Start => start;
    public readonly long End => start + length;
    public readonly long Length => length;

    public void Deconstruct(out long start, out long end)
        => (start, end)
        = (Start, End);

    public readonly long this[long index]
        => Start + index;

    public readonly long IndexOf(long l)
        => Contains(l) ? l - Start : -1;

    public readonly bool Contains(long l)
        => l >= start && l < End;

    public readonly bool Contains(LongRange lr)
        => Start <= lr.Start && End >= lr.End;

    public readonly bool Overlaps(LongRange lr)
        => lr.Start < End && lr.End > start;

    public readonly LongRange Overlay(LongRange lr)
    {
        long newStart = Math.Max(start, lr.Start);
        long newEnd = Math.Min(End, lr.End);
        return new(newStart, newEnd - newStart);
    }

    public readonly LongRange Head(LongRange lr)
    {
        long newStart = Math.Min(start, lr.Start);
        long peakStart = Math.Min(start, lr.Start);
        return new(newStart, peakStart - newStart);
    }

    public readonly LongRange Tail(LongRange lr)
    {
        long newStart = Math.Min(Start, lr.Start);
        //long peakEnd = Math.Max(End, lr.End);
        return new(newStart, start - lr.Start);
    }

    public readonly LongRange Offset(LongRange lr)
    {
        long newStart = Math.Min(Start, lr.Start);
        LongRange overlay = Overlay(lr);
        return new(overlay.Start - newStart, overlay.Length);
    }

    public static LongRange operator +(LongRange a, LongRange b)
        => new(a.Start + b.Start, Math.Min(a.Length, b.Length));

    public readonly IEnumerable<long> Enumerate()
    {
        for (long i = start, e = End; i < e; i++)
            yield return i;
    }
}