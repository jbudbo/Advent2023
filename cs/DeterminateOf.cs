internal readonly struct DeterminateOf<I>
{
    public readonly bool Valid { get; }

    public readonly I Value { get; }

    private DeterminateOf(bool valid, I value)
        => (Valid, Value)
        = (valid, value);

    public void Deconstruct(out bool Valid, out I Value)
        => (Valid, Value)
        = (this.Valid, this.Value);

    public static DeterminateOf<I> Good(I value)
        => new(true, value);

    public static DeterminateOf<I> Bad(I value = default)
        => new(false, value);
}
