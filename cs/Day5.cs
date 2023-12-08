namespace cs;

//internal sealed class Day5 : AdventBase<RangedMap, long>
//{
//    protected override string Day => nameof(Day5);

//    protected override int HeaderLines => 1;

//    private LongRange[] seeds = [];
//    private readonly Dictionary<string, List<RangedMap>> maps = [];
//    private string currentMap = string.Empty;

//    protected override async ValueTask<long> PartOne(IAsyncEnumerable<DataLine<RangedMap>> source, CancellationToken cancellationToken = default)
//    {
//        await foreach(var row in source)
//        {
//            var (_, (valid, data)) = row;
//            if (!valid) continue;

//            maps[currentMap].Add(data);
//        }

//        return default;
//    }

//    protected override async ValueTask<long> PartTwo(IAsyncEnumerable<DataLine<RangedMap>> source, CancellationToken cancellationToken = default)
//    {
//        await foreach (var row in source)
//        {
//            var (_, (valid, data)) = row;
//            if (!valid) continue;

//            maps[currentMap].Add(data);
//        }



//        return default;
//    }

//    protected override void ParseHeader(DataLine<string?> header, bool PartTwo)
//    {
//        var (_, (valid, data)) = header;

//        if (!valid) return;

//        string seedSet = SplitString(data!,2)[1];

//        if (!PartTwo)
//        {
//            seeds = SplitString(seedSet)
//                .Select(long.Parse)
//                .Select(static l => new LongRange(l,1))
//                .ToArray();
//        }
//        else
//        {
//            seeds = SplitString(seedSet)
//                .Select(long.Parse)
//                .Chunk(2)
//                .Select(static l => new LongRange(l[0], l[1]))
//                .ToArray();
//        }
//    }

//    protected override DeterminateOf<RangedMap> ParseData(DataLine<string?> row, bool PartTwo)
//    {
//        var (_, (valid, data)) = row;

//        if (!valid)
//            return DeterminateOf<RangedMap>.Bad();

//        // If this is a map definition, prep our dictionary
//        if (data!.Contains("map"))
//        {
//            string mapHeader = SplitString(data!, 2)[0];
//            maps[mapHeader] = [];
//            currentMap = mapHeader;

//            return DeterminateOf<RangedMap>.Bad();
//        }

//        long[] bits = SplitString(data!, 3).Select(long.Parse).ToArray();

//        RangedMap map = new(bits);
//        return DeterminateOf<RangedMap>.Good(map);
//    }
//}

//internal static partial class Day5Bad
//{
//    public const string P1_EXAMPLE = """
//        seeds: 79 14 55 13

//        seed-to-soil map:
//        50 98 2
//        52 50 48

//        soil-to-fertilizer map:
//        0 15 37
//        37 52 2
//        39 0 15

//        fertilizer-to-water map:
//        49 53 8
//        0 11 42
//        42 0 7
//        57 7 4

//        water-to-light map:
//        88 18 7
//        18 25 70

//        light-to-temperature map:
//        45 77 23
//        81 45 19
//        68 64 13

//        temperature-to-humidity map:
//        0 69 1
//        1 0 69

//        humidity-to-location map:
//        60 56 37
//        56 93 4
//        """;

//    internal static async ValueTask<long> P2(Stream input, CancellationToken cancellationToken = default)
//    {
//        using var rdr = new StreamReader(input);

//        var (seeds, maps) = await ReadInputAsync(rdr, true, cancellationToken);

//        var seedMaps = seeds.Chunk(2)
//            .Select(c => new LongRange(c[0], c[1]))
//            .ToArray();

//        var a = MapDestinations(maps[0], seedMaps)
//            .ToHashSet();

//        var b = MapDestinations(maps[1], a)
//            .ToHashSet();

//        var c = MapDestinations(maps[2], b)
//            .ToHashSet();

//        var d = MapDestinations(maps[3], c)
//            .ToHashSet();

//        var e = MapDestinations(maps[4], d)
//            .ToHashSet();

//        var f = MapDestinations(maps[5], e)
//            .ToHashSet();

//        var g = MapDestinations(maps[6], f)
//            .ToHashSet();

//        var h = g.OrderBy(m => m.Start).ToArray();

//        return 0;
//    }

//    private static IEnumerable<LongRange> MapDestinations(IEnumerable<RangedMap> maps, IEnumerable<LongRange> sources)
//    {
//        //  If a map completely contains a source, map it to a destination and return that
//        //  If a source doesn't overlap a map in at all
//        //      First, check with any other maps before we pass judgement
//        //      If no other map even so much as overlays the source, return the source as it's own destination
//        foreach (var s in sources)
//        {
//            foreach (var map in maps)
//            {
//                var (source, destination) = map;

//                if (source.Contains(s))
//                {
//                    //  Here, the map completely contains the source so we can just work 
//                    //      right through to the corresponding destinations
//                    LongRange offset = source.Offset(s);
//                    yield return destination + offset;
//                    //  Presuming only one source can contain a whole set
//                    break;
//                }

//                //  If we got here, the map does NOT completely engulf the source
//                //      Need to check if there is any overlap at all
//                if (source.Overlaps(s))
//                {
//                    //  If it does, we at least have a partial which we need to deal with
//                    //  First lets identify what mapping we can do by finding what does fall
//                    //      within
//                    LongRange overlay = source.Overlay(s);
//                    //  Anything that is an overlay can be mapped to a destination so let's do so
//                    //      first by offseting our segment
//                    LongRange offset = overlay.Offset(source);
//                    yield return destination + offset;

//                    //  But now we have to deal with whatever didn't fit which we should be able to do 
//                    //      recursively
//                    if (overlay.End < s.End)
//                    {
//                        LongRange outlier = new(overlay.Start, s.End - overlay.Start);
//                        foreach(var x in MapDestinations(maps, new[] { outlier }))
//                        {

//                        }
//                    }
//                }
//            }
//        }
//    }

//    internal static async ValueTask<long> P1(Stream input, CancellationToken cancellationToken = default)
//    {
//        using var rdr = new StreamReader(input);
        
//        var (seeds, maps) = await ReadInputAsync(rdr, cancellationToken: cancellationToken);

//        long[] locations = seeds.Select(maps.Traverse).ToArray();

//        Array.Sort(locations);

//        return locations[0];
//    }

//    private static async ValueTask<(long[], CascadingMap)> ReadInputAsync(StreamReader rdr, bool treatSeedsAsRanges = false, CancellationToken cancellationToken = default)
//    {
//        //Read seeds
//        string? seedLine = await rdr.ReadLineAsync(cancellationToken);

//        IEnumerable<long> eSeeds = seedLine![6..]
//            .Split(' ', DefaultSplitOptions)
//            .Select(long.Parse);

//        await rdr.ReadLineAsync(cancellationToken);

//        //Read soil maps
//        RangedMap[] soilMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] fertilizerMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] waterMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] lightMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] tempMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] humidityMaps = await ReadMapAsync(rdr, cancellationToken);
//        RangedMap[] locationMaps = await ReadMapAsync(rdr, cancellationToken);

//        return (eSeeds.ToArray(), new(soilMaps, fertilizerMaps, waterMaps, lightMaps, tempMaps, humidityMaps, locationMaps));
//    }

//    private static async ValueTask<RangedMap[]> ReadMapAsync(StreamReader rdr, CancellationToken cancellationToken)
//    {
//        // Jump the header
//        await rdr.ReadLineAsync(cancellationToken);

//        async IAsyncEnumerable<RangedMap> getDataLines([EnumeratorCancellation] CancellationToken cancellationToken = default)
//        {
//            string? line = await rdr.ReadLineAsync(cancellationToken);
//            while (!string.IsNullOrWhiteSpace(line))
//            {
//                yield return new(line
//                    .Split(' ', DefaultSplitOptions)
//                    .Select(long.Parse)
//                    .ToArray());

//                line = await rdr.ReadLineAsync(cancellationToken);
//            }
//        }

//        List<RangedMap> maps = [];
//        await foreach(var map in getDataLines(cancellationToken))
//            maps.Add(map);

//        return [.. maps];
//    }

//    private const StringSplitOptions DefaultSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
//}

//internal readonly struct CascadingMap(params RangedMap[][] maps)
//{

//    public readonly RangedMap[] this[int index] => maps[index];

//    public readonly long Traverse(long seed)
//    {
//        ReadOnlySpan<RangedMap[]> buffer = maps;
//        long d = seed;
//        for (int i = 0, j = buffer.Length; i < j; i++)
//        {
//            d = DetermineDestination(buffer[i], d);
//        }

//        return d;
//    }

//    private static long DetermineDestination(in ReadOnlySpan<RangedMap> range, long s)
//    {
//        int i = -1, j = range.Length;
//        while (++i < j)
//        {
//            long d = range[i].GetDestinationFor(s);
//            if (d > -1)
//                return d;
//        }
//        return s;
//    }
//}

[DebuggerStepThrough]
[DebuggerDisplay("{Destination} - {Source}")]
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