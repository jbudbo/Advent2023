namespace cs;

using Point = (int X, int Y);

internal class Day10 : AdventBase<IEnumerable<MapSymbol>>
{
    protected override string Day => nameof(Day10);

    protected override Uof<IEnumerable<MapSymbol>> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (v, d)) = row;

        if (!v) return Union.Fail<IEnumerable<MapSymbol>>();

        return Union.Pass(d!.Select(static c => c.AsSymbol()));
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<IEnumerable<MapSymbol>>> source, CancellationToken cancellationToken = default)
    {
        List<MapRow> map = [];

        await foreach (DataLine<IEnumerable<MapSymbol>> row in source)
        {
            var (ln, (v, data)) = row;
            if (!v) continue;

            map.Add(new(ln, data));
        }

        Map m = new(map);

        var candidates = m.GetNeighborsOf(m.Origin).Where(static t => t.Item2 is not MapSymbol.Gnd).ToArray();

        var a = m.GetNext(m.Origin, candidates[0].Item1);

        var b = m.GetNext(a.Item1, m.Origin);

        long fullTrip = 2;
        while (!cancellationToken.IsCancellationRequested)
        {
            var c = m.GetNext(b.Item1, a.Item1);
            if (c.Item2 is MapSymbol.Start)
                break;
            fullTrip++;
            a = b;
            b = c;
            try
            {
                //await Task.Delay(delay, cancellationToken);
            }
            catch { }
        }
        Answer = (fullTrip + 1) / 2;
    }

    private void W_Update(double obj)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask PartTwo(IAsyncEnumerable<DataLine<IEnumerable<MapSymbol>>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

readonly struct Map
{
    public readonly MapRow[] _rows;

    public Map(IEnumerable<MapRow> rows)
        => _rows = rows.ToArray();

    public readonly MapSymbol this[Point p] => _rows[p.Y][p.X];

    public readonly (Point, MapSymbol) GetNext(Point origin, Point last)
    {
        MapSymbol ms = this[origin];

        try
        {
            return GetNeighborsOf(origin)
                .First(t => t.Item2 is not MapSymbol.Gnd
                    && !(t.Item1.X == last.X && t.Item1.Y == last.Y)
                    && ms switch
                    {
                        MapSymbol.UR when t.Item1.X == origin.X => t.Item1.Y > origin.Y, //heading Down from upper right
                        MapSymbol.UR when t.Item1.Y == origin.Y => t.Item1.X < origin.X, //heading Left from upper right
                        MapSymbol.UL when t.Item1.X == origin.X => t.Item1.Y > origin.Y, //heading Down from upper left
                        MapSymbol.UL when t.Item1.Y == origin.Y => t.Item1.X > origin.X, //heading Right from upper left
                        MapSymbol.LL when t.Item1.X == origin.X => t.Item1.Y < origin.Y, //heading Up from lower left
                        MapSymbol.LL when t.Item1.Y == origin.Y => t.Item1.X > origin.X, //heading Right from lower left
                        MapSymbol.LR when t.Item1.X == origin.X => t.Item1.Y < origin.Y, //heading Up from lower right
                        MapSymbol.LR when t.Item1.Y == origin.Y => t.Item1.X < origin.X, //heading Left from lower right
                        MapSymbol.Horiz => t.Item1.Y == origin.Y,
                        MapSymbol.Vert => t.Item1.X == origin.X,
                        MapSymbol.Start => true,
                        _ => false
                    });
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public readonly IEnumerable<(Point, MapSymbol)> GetNeighborsOf(Point p)
    {
        if (p.Y > 0)
        {
            Point c = (p.X, p.Y - 1);
                yield return (c, _rows[p.Y - 1][p.X]);
        }

        if (p.Y + 1 < _rows.Length)
        {
            Point c = (p.X, p.Y + 1);
                yield return (c, _rows[p.Y + 1][p.X]);
        }

        var (ileft, iright) = _rows[p.Y].GetNeighbors(p.X);

        if (ileft)
            yield return ((ileft.Value, p.Y), _rows[p.Y][ileft.Value]);

        if (iright)
            yield return ((iright.Value, p.Y), _rows[p.Y][iright.Value]);
    }

    public readonly Point Origin
    {
        get
        {
            ReadOnlySpan<MapRow> r = _rows;
            int y = 0, yj = _rows.Length;
            while (y < yj)
            {
                MapRow row = r[y++];
                if (row.HasStart)
                {
                    return (row.StartX, y - 1);
                }
            }
            throw new UnreachableException();
        }
    }
}

[DebuggerDisplay("{DebugView}")]
readonly struct MapRow
{
    private readonly int _index;
    private readonly MapSymbol[] _points;
    public MapRow(int rowNumber, IEnumerable<MapSymbol> points)
        => (_index, _points)
        = (rowNumber, points.ToArray());

    public readonly MapSymbol this[int i] => _points[i];

    public readonly (Uof<int>, Uof<int>) GetNeighbors(int p) => (
        p is 0 ? Union.Fail<int>() : Union.Pass(p - 1),
        p + 1 == _points.Length ? Union.Fail<int>() : Union.Pass(p + 1)
    );

    public readonly int Length => _points.Length;

    public readonly bool HasStart => StartX > -1;

    public readonly int StartX => Array.IndexOf(_points, MapSymbol.Start);

    private string DebugView => $"{_index}: {string.Join('-', _points)}";
}

enum MapSymbol : byte
{
    /// <summary>
    /// <code>|</code>
    /// </summary>
    Vert = 1,
    /// <summary>
    /// <code>-</code>
    /// </summary>
    Horiz = 2,
    /// <summary>
    /// <code>.</code>
    /// </summary>
    Gnd = 3,
    /// <summary>
    /// <code>S</code>
    /// </summary>
    Start = 4,
    /// <summary>
    /// <code>L</code>
    /// </summary>
    LL = 5,
    /// <summary>
    /// <code>J</code>
    /// </summary>
    LR = 6,
    /// <summary>
    /// <code>7</code>
    /// </summary>
    UR = 7,
    /// <summary>
    /// <code>F</code>
    /// </summary>
    UL = 8
}

static class DayExtensions
{
    internal static MapSymbol AsSymbol(this char c) => c switch
    {
        '.' => MapSymbol.Gnd,
        '|' => MapSymbol.Vert,
        '-' => MapSymbol.Horiz,
        's' or 'S' => MapSymbol.Start,
        'l' or 'L' => MapSymbol.LL,
        'j' or 'J' => MapSymbol.LR,
        '7' => MapSymbol.UR,
        'f' or 'F' => MapSymbol.UL,
        _ => throw new UnreachableException()
    };

    internal static char AsChar(this MapSymbol s) => s switch
    {
        MapSymbol.Gnd => ' ',
        MapSymbol.Vert => '|',
        MapSymbol.Horiz => '-',
        MapSymbol.Start => '@',
        MapSymbol.UL => 'F',
        MapSymbol.UR => '7',
        MapSymbol.LL => 'L',
        MapSymbol.LR => 'J',
        _ => throw new UnreachableException()
    };
}