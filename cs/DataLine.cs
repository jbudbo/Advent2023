internal readonly struct DataLine<I>(int lineNumber, DeterminateOf<I> data)
{
    public readonly int LineNumber => lineNumber;

    public readonly DeterminateOf<I> Data => data;

    public void Deconstruct(out int LineNumber, out DeterminateOf<I> Data)
        => (LineNumber, Data)
        = (this.LineNumber, this.Data);
}
