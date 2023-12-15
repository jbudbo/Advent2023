
using cs;

internal class Day14 : AdventBase<char[]>
{
    protected override string Day => nameof(Day14);

    protected override Uof<char[]> ParseData(DataLine<string?> row, bool PartTwo)
        => row.Data.Valid ? Union.Pass(row.Data.Value!.ToArray()) : Union.Fail<char[]>();

    protected override async ValueTask PartOne(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        List<char[]> map = [];
        await foreach (var (_, (_, rowData)) in source)
        {
            map.Add(rowData);
        }

        Tilt(map);

        for (int i = 0, j = map.Count, v = j; i < j; i++, v--)
        {
            int rocks = map[i].Count(static c => c is 'O');
            Answer += rocks * v;
        }
    }

    protected override async ValueTask PartTwo(IAsyncEnumerable<DataLine<char[]>> source, CancellationToken cancellationToken = default)
    {
        //const long cycleCount = 1_000_000_000;
        const long cycleCount = 3;

        List<char[]> map = [];
        await foreach (var (_, (_, rowData)) in source)
        {
            map.Add(rowData);
        }

        //North
        Tilt(map);

        //West
        map = RotateAndTilt(map);

        //South
        map = RotateAndTilt(map);

        //East
        map = RotateAndTilt(map);

        foreach(var l in map)
        {
            Console.WriteLine(string.Concat(l));
        }
    }

    private static List<char[]> RotateAndTilt(List<char[]> map) 
    {
        List<char[]> newMap = [];
        for (int i = 0, j = map[0].Length; i < j; i++)
        {
            newMap.Add(map.Select(c => c[i]).ToArray());
        }
        Tilt(newMap);
        return newMap;
    }

    private static void Tilt(List<char[]> data)
    {
        bool doneRolling;

        do
        {
            doneRolling = true;
            for (int i = data.Count - 1; i >= 0; i--)
            {
                char[] line = data[i];
                var s = string.Concat(line);
                for (int x = 0, y = data[i].Length; x < y; x++)
                {
                    char sprite = data[i][x];
                    char nSprint = i >= 1 ? data[i - 1][x] : '\0';

                    switch (sprite)
                    {
                        case '#' or '.': // No rolling
                            break;
                        case 'O' when nSprint is '.':
                            data[i - 1][x] = 'O';
                            data[i][x] = '.';
                            doneRolling = false;
                            break;
                        default:

                            break;
                    }
                }
            }
        } while (!doneRolling);
    }
}
