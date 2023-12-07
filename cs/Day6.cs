namespace cs;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Round = (long Time, long RecordDistance);

internal static partial class Day6
{
    internal static async ValueTask<long> P2(Stream input, CancellationToken cancellationToken = default)
    {
        using StreamReader rdr = new(input);

        Round race = await ReadAdjustedInput(rdr, cancellationToken);

        var (a,b,o) = SolveQuadratic(race);

        return a - b + o;
    }

    internal static async ValueTask<long> P1(Stream input, CancellationToken cancellationToken = default)
    {
        using StreamReader rdr = new(input);

        long total = 1;
        await foreach(Round r in ReadInput(rdr, cancellationToken))
        {
            var (a, b, o) = SolveQuadratic(r);

            total *= (a - b + o);
        }

        return total;
    }

    private static (long, long, long) SolveQuadratic(Round r)
    {
        // 0 = a(X^2) - bX - c
        //  In our case a is always 1, b is negative time, c is record distance
        long h = -r.Time * -r.Time;
        long i = 4 * r.RecordDistance;

        double s = Math.Sqrt(h - i);

        long offset = (long)Math.Ceiling(s % 1.00);
        //the next whole number is a loss
        long a = (long)((r.Time + s) / 2);
        //the next whole number is a win
        long b = (long)Math.Floor((r.Time - s) / 2) + 1;

        return (a, b, offset);
    }

    private static async ValueTask<Round> ReadAdjustedInput(StreamReader rdr, CancellationToken cancellationToken = default)
    {
        string timeLine = await rdr.ReadLineAsync(cancellationToken);
        string fullTime = timeLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]
            .Replace(" ", string.Empty);

        string distanceLine = await rdr.ReadLineAsync(cancellationToken);
        string fullRecord = distanceLine.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]
            .Replace(" ", string.Empty);

        return (long.Parse(fullTime), long.Parse(fullRecord));
    }

    private static async IAsyncEnumerable<Round> ReadInput(StreamReader rdr
        , [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string timeLine = await rdr.ReadLineAsync(cancellationToken);
        var times = timeLine
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .Skip(1);

        string distanceLine = await rdr.ReadLineAsync(cancellationToken);
        var records = distanceLine
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse)
            .Skip(1);

        foreach (var r in times.Zip(records))
            yield return r;
    }
}
