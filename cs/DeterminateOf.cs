[DebuggerDisplay("{DebugView}")]
[DebuggerStepThrough]
internal readonly struct Uof<I>
{
    public readonly bool Valid { get; }

    public readonly I Value { get; }

    private Uof(bool valid, I value)
        => (Valid, Value)
        = (valid, value);

    public void Deconstruct(out bool Valid, out I Value)
        => (Valid, Value)
        = (this.Valid, this.Value);

    public static Uof<I> Good(I value)
        => new(true, value);

    public static Uof<I> Bad(I value = default)
        => new(false, value);

    public static implicit operator bool(Uof<I> uof) => uof.Valid;

    private string DebugView
        => $"({Valid}) {(Valid ? Value.ToString() : string.Empty)}";
}

[DebuggerStepThrough]
internal static class Union
{
    public static Uof<T> Pass<T>(T t)
        => Uof<T>.Good(t);

    public static Uof<T> Fail<T>()
        => Uof<T>.Bad();
}