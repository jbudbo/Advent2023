using System.Runtime.CompilerServices;

[DebuggerStepThrough]
internal abstract class AdventBase<I> : AdventBase<I, long> { }

[DebuggerStepThrough]
internal abstract class AdventBase<I,A> 
{
    public delegate ValueTask DayCallback(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);

    private const string DATA_BASE_PATH = "Data";

    protected abstract string Day { get; }

    protected virtual int HeaderLines => 0;

    protected A Answer = default;

    protected abstract ValueTask PartOne(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);
    protected abstract ValueTask PartTwo(IAsyncEnumerable<DataLine<I>> source
        , CancellationToken cancellationToken = default);

    protected abstract Uof<I> ParseData(DataLine<string?> row, bool PartTwo);

    protected virtual void ParseHeader(DataLine<string?> header, bool PartTwo) { }

    public async ValueTask<A> PartOne(CancellationToken cancellationToken = default)
    { 
        await ReadForDay(PartOne, false, cancellationToken);
        return Answer;
    }

    public async ValueTask<A> PartTwo(CancellationToken cancellationToken = default)
    { 
        await ReadForDay(PartTwo, true, cancellationToken);
        return Answer;
    }

    private async ValueTask ReadForDay(DayCallback day, bool PartTwo, CancellationToken cancellationToken = default)
    {
        using Stream fileStream = File.OpenRead($"{DATA_BASE_PATH}/{Path.ChangeExtension(Day,"data")}");
        using StreamReader reader = new(fileStream);

        IAsyncEnumerable<DataLine<I>> input = ReadInput(reader, PartTwo, cancellationToken);

        await day(input, cancellationToken);
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
                ParseHeader(new(lineNumber++, Uof<string?>.Bad()), PartTwo);
            else
                ParseHeader(new(lineNumber++, Uof<string?>.Good(line)), PartTwo);
        }

        while (!rdr.EndOfStream)
        {
            line = await rdr.ReadLineAsync(cancellationToken);
            Uof<string?> entry;
            if (string.IsNullOrWhiteSpace(line))
                entry = Uof<string?>.Bad();
            else
                entry = Uof<string?>.Good(line);

            Uof<I> i = ParseData(new(lineNumber, entry), PartTwo);

            yield return new(lineNumber++, i);
        }
    }

    protected static string[] SplitString(string input, int? fieldCount = null, char separator = ' ')
        => fieldCount is null
        ? input.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
        : input.Split(separator, fieldCount.Value, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}