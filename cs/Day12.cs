using cs;

internal class Day12 : AdventBase<char[]>
{
    protected override string Day => nameof(Day12);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
    {
        if (!row.Data.Valid)
            return Union.Fail<char[]>();

        return Union.Pass(row.Data.Value.ToCharArray());
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        await foreach(var (_, (valid, data)) in source)
        {
            if (!valid) continue;

            // ? - -1
            // . - 0
            // # - 1
            var (points, sizes) = ParseRow(data);

            int potentialCandidates = points.Length - sizes.Sum() - sizes.Length + 2;
            
            for (int i = 0; i < potentialCandidates; i++)
            {

            }
        }
    }

    private static IEnumerable<Range> EnumerateWindows(int start, int len, int last)
    {
        int end = start + len;
        while (end <= last)
        {
            yield return new Range(start++, end++);
        }
    }

    protected override ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private static (int[] space, int[] sizes) ParseRow(ReadOnlySpan<char> source)
    {
        Span<Range> ranges = stackalloc Range[source.Length];
        int parts = source.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        int[] space = [..source[ranges[0]].As(static c => c switch
        {
            '#' => 1,
            '.' => 0,
            _ => -1
        })];

        ReadOnlySpan<char> countBuffer = source[ranges[1]];

        parts = source[ranges[1]].Split(ranges, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int[] counts = new int[parts];
        int i = 0;
        while (i < parts)
        {
            counts[i] = countBuffer[ranges[i++]][0] - '0';
        }

        return (space, counts);
    }
}
