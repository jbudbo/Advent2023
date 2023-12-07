using System.Runtime.CompilerServices;

[DebuggerStepThrough]
internal abstract class AdventBase<I,A> where I : struct
{
    public delegate ValueTask<A> DayCallback(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);

    private const string DATA_BASE_PATH = "Data";

    protected abstract string Day { get; }

    protected virtual int HeaderLines => 0;

    protected abstract ValueTask<A> PartOne(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);
    protected abstract ValueTask<A> PartTwo(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);

    protected virtual DeterminateOf<I> ParseData(DataLine<string?> row, bool PartTwo) => default;

    protected virtual void ParseHeader(DataLine<string?> header, bool PartTwo) { }

    public ValueTask<A> PartOne(CancellationToken cancellationToken = default)
        => ReadForDay(PartOne, false, cancellationToken);

    public ValueTask<A> PartTwo(CancellationToken cancellationToken = default)
        => ReadForDay(PartTwo, true, cancellationToken);

    private async ValueTask<A> ReadForDay(DayCallback day, bool PartTwo, CancellationToken cancellationToken = default)
    {
        using Stream fileStream = File.OpenRead($"{DATA_BASE_PATH}/{Path.ChangeExtension(Day,"data")}");
        using StreamReader reader = new(fileStream);

        IAsyncEnumerable<DataLine<I>> input = ReadInput(reader, PartTwo, cancellationToken);

        return await day(input, cancellationToken);
    }

    private async IAsyncEnumerable<DataLine<I>> ReadInput(StreamReader rdr
        , bool PartTwo = false
        , [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int lineNumber = 0;
        string? line;

        while (!rdr.EndOfStream && lineNumber < HeaderLines)
        {
            line = await rdr.ReadLineAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(line))
                ParseHeader(new(lineNumber++, DeterminateOf<string?>.Bad()), PartTwo);
            else
                ParseHeader(new(lineNumber++, DeterminateOf<string?>.Good(line)), PartTwo);
        }

        while (!rdr.EndOfStream)
        {
            line = await rdr.ReadLineAsync(cancellationToken);
            DeterminateOf<string?> entry;
            if (string.IsNullOrWhiteSpace(line))
                entry = DeterminateOf<string?>.Bad();
            else
                entry = DeterminateOf<string?>.Good(line);

            DeterminateOf<I> i = ParseData(new(lineNumber, entry), PartTwo);

            yield return new(lineNumber++, i);
        }
    }

    protected static string[] SplitString(string input, int? fieldCount = null, char separator = ' ')
        => fieldCount is null
        ? input.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        : input.Split(separator, fieldCount.Value, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}