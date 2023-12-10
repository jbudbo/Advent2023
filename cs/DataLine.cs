internal readonly struct DataLine<I>(int lineNumber, Uof<I> data)
{
    public readonly int LineNumber => lineNumber;

    public readonly Uof<I> Data => data;

    public void Deconstruct(out int LineNumber, out Uof<I> Data)
        => (LineNumber, Data)
        = (this.LineNumber, this.Data);
}
