using Galaxy = (int X, int Y);
using cs;
using System.Diagnostics.CodeAnalysis;

internal class Day11 : AdventBase<char[]>
{
    protected override string Day => nameof(Day11);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
    {
        if (row.Data.Valid)
            return Union.Pass(row.Data.Value.ToCharArray());

        return Union.Fail<char[]>();
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        await CalculateAnswer(source, 2);
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        await CalculateAnswer(source, 1_000_000);
    }

    private async Task CalculateAnswer(IAsyncEnumerable<DataLine<char[]>> source, int modifier)
    {
        List<Galaxy> universe = [];
        await foreach (var (y, (valid, row)) in source)
        {
            if (!valid) continue;

            foreach (int x in row.IndexesOf('#'))
            {
                universe.Add((x, y));
            }
        }

        Answer = SumMinDistances(universe.ToArray(), modifier);
    }

    private static long SumMinDistances(Galaxy[] galaxies, int modifier)
    {
        long a = 0;

        var (xSpaces, ySpaces) = FindEmpties(galaxies);

        int i = 0, j = galaxies.Length;

        HashSet<Galaxy> visited = new(j, GalaxyComparer.Instance);
        while (i < j)
        {
            Galaxy o = galaxies[i];
            visited.Add(o);

            bool notOrigin(Galaxy g)
                => g.X != o.X && g.Y != o.Y;

            //  Need to identify any spaces between the origin galaxy, the the next
            foreach (Galaxy g in galaxies.Except(visited, GalaxyComparer.Instance))
            {
                int xDistance = WeightsBetween(o.X, g.X, xSpaces, modifier)
                    .Sum();

                int yDistance = WeightsBetween(o.Y, g.Y, ySpaces, modifier)
                    .Sum();

                long distance = xDistance + yDistance;

                a += distance;
            }

            i++;
        }

        return a;
    }

    [DebuggerStepThrough]
    private class GalaxyComparer : IEqualityComparer<Galaxy>
    {
        public static readonly GalaxyComparer Instance = new();

        public bool Equals((int X, int Y) x, (int X, int Y) y)
            => x.X == y.X && x.Y == y.Y;

        public int GetHashCode([DisallowNull] (int X, int Y) obj)
            => (obj.X * obj.Y).GetHashCode();
    }

    private static IEnumerable<int> WeightsBetween(int a, int b, int[] modifierIndexes, int modifier)
    {
        for (int i = Math.Min(a, b) + 1, j = Math.Max(a, b); i <= j; i++)
        {
            yield return Array.IndexOf(modifierIndexes, i) is not -1
                ? modifier : 1;
        }
    }

    private static (int[] X, int[] Y) FindEmpties(Galaxy[] galaxies)
    {
        HashSet<int> x = [], y = [];

        int w = 0 , h = 0 ;
        foreach(var (gx, gy) in galaxies)
        {
            x.Add(gx);
            y.Add(gy);

            if (gx > w) w = gx;
            if (gy > h) h = gy;
        }

        return 
            (
                Enumerable.Range(0, w).Except(x).ToArray(),
                Enumerable.Range(0, h).Except(y).ToArray()
            );
    }
}
