internal class Day13 : AdventBase<char[]>
{
    protected override string Day => nameof(Day13);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
    {
        var (_, (v, r)) = row;
        if (!v)
            return Union.Fail<char[]>();

        return Union.Pass(r!.ToCharArray());
    }

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        State state = new();

        long hz = 0, vt = 0;

        await foreach (var (_, (v, dataRow)) in source)
        {
            //  If this line is not "valid" it's an empty row between inputs
            if (!v)
            {
                var (horiz, vert) = state.MirrorValue;
                hz += horiz;
                vt += vert;
                state.Clear();
                continue;
            }
            state.AddData(dataRow);
        }
        {
            var (horiz, vert) = state.MirrorValue;
            hz += horiz;
            vt += vert;
        }
        Answer = (vt * 100) + hz;
    }

    private static void InspectRow(ReadOnlySpan<char> dataRow, List<int> candidates)
    {
        for (int i = 0, j = dataRow.Length - 1; i < j; i++)
        {
            char a = dataRow[i];
            char b = dataRow[i + 1];

            //  If they don't match, no point in continuing
            if (a != b) continue;

            //  If they do match however, now we need to work outward from here
            //      checking matches if/til we hit an edge
            bool metEdge = true;
            for (int x = i - 1, y = i + 2; x >= 0 && y < dataRow.Length; x--, y++)
            {
                char c = dataRow[x];
                char d = dataRow[y];

                //  If they don't match, and we're not at the edge,
                //      then we didn't make it and we're done
                if (c != d)
                {
                    metEdge = false;
                    break;
                }
            }

            //  If we hit a mirror but it didn't make it to the edge, move on
            if (!metEdge)
                continue;

            //  However, if we did, this is a potential index to check for the next line
            //      and I'm assuming there could be many as we progress through the next
            //      lines. So we'll bank this one and keep check
            candidates.Add(i);
        }
    }

    protected override ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    sealed class State
    {
        private readonly bool _rotation;
        private readonly List<char[]> data = [];

        private int[]? candidates = null;

        public bool ConfirmedNoMirror => candidates is { Length: 0 };

        public bool MirrorFound => candidates is { Length: 1 };

        public State()
            => _rotation = false;

        private State(bool rotation)
            => _rotation = rotation;

        private long Value => MirrorFound ? candidates![0] + 1 : 0;

        private long RotationValue => ValueRotation();

        public (long horiz, long vert) MirrorValue
            => (Value, RotationValue);

        public void Clear()
        {
            candidates = null;
            data.Clear();
        }

        private long ValueRotation()
        {
            if (_rotation) return 0;

            State state = new(true);
            for (int i = 0, j = data[0].Length; i < j; i++)
            {
                char[] row = data.Select(r => r[i]).ToArray();
                state.AddData(row);
            }

            if (state.MirrorFound)
                return state.Value;

            return 0;
        }

        public void AddData(char[] row)
        {
            data.Add(row);

            //  If we've never pulled together any candidates, then
            //      we're likely on the first row and need to get started
            if (candidates is null)
            {
                List<int> potentials = [];
                InspectRow(row, potentials);
                candidates = [.. potentials];
                return;
            }

            //  If our candidates are 0, then we pulled together a list
            //  and removed this set over time indicating there are no
            //  horizontal mirrors and therefore we need not continue
            if (ConfirmedNoMirror)
                return;

            //  Otherwise, we need to check this row and see if our 
            //      candidates hold true here as well. If they don't
            //      we need to remove them from the running
            List<int> potential = [.. candidates];
            for (int i = 0, j = potential.Count; i < j; i++)
            {
                for (int x = potential[i], y = x + 1; x >= 0 && y < row.Length; x--, y++)
                {
                    char c = row[x];
                    char d = row[y];

                    //  If they don't match, and we're not at the edge,
                    //      then we didn't make it and we're done
                    if (c != d)
                    {
                        potential.RemoveAt(i--);
                        j--;
                        break;
                    }
                }
            }
            candidates = [.. potential];
        }

        private static void InspectRow(ReadOnlySpan<char> dataRow, List<int> candidates)
        {
            for (int i = 0, j = dataRow.Length - 1; i < j; i++)
            {
                char a = dataRow[i];
                char b = dataRow[i + 1];

                //  If they don't match, no point in continuing
                if (a != b) continue;

                //  If they do match however, now we need to work outward from here
                //      checking matches if/til we hit an edge
                bool metEdge = true;
                for (int x = i - 1, y = i + 2; x >= 0 && y < dataRow.Length; x--, y++)
                {
                    char c = dataRow[x];
                    char d = dataRow[y];

                    //  If they don't match, and we're not at the edge,
                    //      then we didn't make it and we're done
                    if (c != d)
                    {
                        metEdge = false;
                        break;
                    }
                }

                //  If we hit a mirror but it didn't make it to the edge, move on
                if (!metEdge)
                    continue;

                //  However, if we did, this is a potential index to check for the next line
                //      and I'm assuming there could be many as we progress through the next
                //      lines. So we'll bank this one and keep check
                candidates.Add(i);
            }
        }
    }
}
