namespace cs;

internal static class AdditionalExtensions
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
}