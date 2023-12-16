namespace cs;

internal static partial class AdditionalExtensions
{
    public static IEnumerable<(T,T)> Window<T>(this IEnumerable<T> source)
    {
        T[] set = source.ToArray();

        int i = 0, j = set.Length;
        while (i + 1 < j)
        {
            yield return (set[i++], set[i]);
        }
    }

    public static IEnumerable<int> IndexesOf<T>(this IEnumerable<T> source, T t)
    {
        int i = 0;
        foreach(T instance in source)
        {
            if (instance.Equals(t))
                yield return i;

            i++;
        }
    }

    public static ReadOnlySpan<O> As<T,O>(this ReadOnlySpan<T> source
        , Func<T, O> transform)
    {
        int i = 0, j = source.Length;
        Span<O> buffer = new O[j];
        while (i < j) buffer[i] = transform(source[i++]);
        return buffer;
    }

}