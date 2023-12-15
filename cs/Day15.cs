
using System.Collections.Specialized;

internal class Day15 : AdventBase<char[]>
{
    protected override string Day => nameof(Day15);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
    {
        return row.Data.Valid
            ? Union.Pass(row.Data.Value!.ToCharArray())
            : Union.Fail<char[]>();
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        long value = 0;
        await foreach (var (_,(valid, rowData)) in source)
        {
            foreach(char c in rowData)
            {
                if (c is ',')
                {
                    //reset
                    Answer += value;
                    value = 0;
                    continue;
                }

                value += c;
                value *= 17;
                value %= 256;
            }
        }
        Answer += value;
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        Dictionary<long, List<(string, int)>> boxes = [];
        char[] buffer = new char[20];
        int index = 0;
        await foreach (var (_, (valid, rowData)) in source)
        {
            foreach (char c in rowData)
            {
                if (c is ',')
                {
                    //reset
                    var (lens, additive, length) = Parse(buffer.AsSpan()[..index]);
                    long box = 0;
                    foreach (char l in lens)
                    {
                        box += l;
                        box *= 17;
                        box %= 256;
                    }

                    if (!boxes.ContainsKey(box))
                        boxes[box] = [];

                    if (!additive)
                        boxes[box].RemoveAll(t => t.Item1 == lens);
                    else
                    {
                        int idx = boxes[box].FindIndex(t => t.Item1 == lens);
                        if (idx > -1)
                        {
                            boxes[box][idx] = (lens, length);
                        }
                        else
                            boxes[box].Add((lens, length));
                    }

                    index = 0;
                    continue;
                }

                buffer[index++] = c;
            }
        }
        {
            var (lens, additive, length) = Parse(buffer.AsSpan()[..index]);
            long box = 0;
            foreach (char l in lens)
            {
                box += l;
                box *= 17;
                box %= 256;
            }

            if (!boxes.ContainsKey(box))
                boxes[box] = [];

            if (!additive)
                boxes[box].RemoveAll(t => t.Item1 == lens);
            else
            {
                int idx = boxes[box].FindIndex(t => t.Item1 == lens);
                if (idx > -1)
                {
                    boxes[box][idx] = (lens, length);
                }
                else
                    boxes[box].Add((lens, length));
            }
        }

        foreach(var (b, ll) in boxes)
        {
            for (int i = 1, j = ll.Count; i <= j; i++)
            {
                var (_, v) = ll[i-1];

                long l = (b + 1) * i * v;
                Answer += l;
            }
        }
    }

    private static readonly char[] delims = ['=', '-'];
    private (string lens, bool remove, int val) Parse(ReadOnlySpan<char> chars)
    {
        Span<Range> buffer = stackalloc Range[2];
        int bits = chars.SplitAny(buffer, delims, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Range b = buffer[0];
        bool additive = bits > 1;
        return (new (chars[b]), additive, additive ? int.Parse(chars[buffer[1]]) : -1);
    }
}
