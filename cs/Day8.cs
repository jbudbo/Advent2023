using DirectionalCoordinate = (string L, string R);
namespace cs;

internal sealed class Day8 : AdventBase<KeyValuePair<string, DirectionalCoordinate>>
{
    protected override string Day => nameof(Day8);

    protected override int HeaderLines => 2;

    private char[] _directions = [];

    private readonly Dictionary<string, DirectionalCoordinate> _coordinates = [];

    private readonly HashSet<string> _origins = [];

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<KeyValuePair<string, DirectionalCoordinate>>> source, CancellationToken cancellationToken = default)
    {
        await foreach (DataLine<KeyValuePair<string, DirectionalCoordinate>> line in source)
        {
            var (_, (valid, data)) = line;

            if (!valid) continue;

            _coordinates.Add(data.Key, data.Value);
        }

        int j = _directions.Length;

        string currentStep = "AAA";
        while (currentStep != "ZZZ")
        {
            int i = 0;
            while (i < j)
            {
                currentStep = _directions[i++] switch
                {
                    'L' => _coordinates[currentStep].L,
                    'R' => _coordinates[currentStep].R,
                    _ => throw new UnreachableException()
                };
                Answer++;
            }
        }
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<KeyValuePair<string, DirectionalCoordinate>>> source, CancellationToken cancellationToken = default)
    {
        await foreach (DataLine<KeyValuePair<string, DirectionalCoordinate>> line in source)
        {
            var (_, (valid, data)) = line;

            if (!valid) continue;

            _coordinates.Add(data.Key, data.Value);

            if (data.Key.EndsWith('A'))
                _origins.Add(data.Key);
        }

        //  Each Node will have a number of steps needed to get to 
        //      the end. We'll accrue them here
        Dictionary<string, (long, string)> nodeWeights = [];

        Queue<(int,string)> nodesToEvaluate = new(_origins.Select(static o => (0, o)));

        //  We need to accrue weights per each starting point
        //      However we can't just assume we're done after one round
        //      Each Z can start us again somewhere else
        while (nodesToEvaluate.TryDequeue(out var t))
        {
            var (dIndex, origin) = t;

            if (nodeWeights.ContainsKey(origin))
                continue;

            int steps = 0;

            string current = origin;
            string next = string.Empty;
            while (current[^1] is not 'Z')
            {
                int d = dIndex;
                for (int j = _directions.Length; d < j; d++)
                {
                    char dir = _directions[d];
                    current = dir switch
                    {
                        'L' => _coordinates[current].L,
                        'R' => _coordinates[current].R,
                        _ => throw new UnreachableException()
                    };

                    steps++;
                    if (current[^1] is 'Z')
                    {
                        //  We need to try and prepare for the next step to traverse
                        int nextd = (d + 1) % _directions.Length;
                        dir = _directions[nextd];
                        next = dir switch
                        {
                            'L' => _coordinates[current].L,
                            'R' => _coordinates[current].R,
                            _ => throw new UnreachableException()
                        };

                        nodesToEvaluate.Enqueue((d, next));

                        break;
                    }
                }
                dIndex = 0;
            }

            nodeWeights[origin] = (steps, next);
        }

        var weights = nodeWeights.Take(_origins.Count)
            .Select(static w => w.Value.Item1)
            .ToArray();

        Answer = 1;
        for (int i = 0, j = _origins.Count; i < j; i++)
        {
            Answer = LCM(weights[i], Answer);
        }
    }

    private static long LCM(long a, long b)
        => a * b / GCD(a, b);

    private static long GCD(long a, long b)
    {
        while (b > 0)
        {
            long x = b;
            b = a % b;
            a = x;
        }

        return a;
    }

    protected override Uof<KeyValuePair<string, DirectionalCoordinate>> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (valid, data)) = row;

        if (!valid) return Uof<KeyValuePair<string, DirectionalCoordinate>>.Bad();

        ReadOnlySpan<string> parts = SplitString(data, 2, '=');
        ReadOnlySpan<string> lr = SplitString(parts[1], 2, ',');

        DirectionalCoordinate c = (lr[0][1..], lr[1][..^1]);
        return Uof<KeyValuePair<string, DirectionalCoordinate>>.Good(new(parts[0], c));
    }

    protected override void ParseHeader(DataLine<string?> header, bool PartTwo)
    {
        if (!header.Data.Valid)
            return;

        _directions = header.Data.Value!.ToCharArray();
    }
}
