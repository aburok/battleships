public static class CollectionExtensions
{
    public static IEnumerable<T[]> SplitBy<T>(this T[] array, int bucketSize)
    {
        for (var i = 0; i < array.Length / bucketSize; i++)
        {
            yield return array.Skip(i * bucketSize).Take(bucketSize).ToArray();
        }
    }
}