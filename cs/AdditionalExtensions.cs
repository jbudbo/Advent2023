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
}