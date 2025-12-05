namespace MarkGravestock.AdventOfCode2025.Common;

public static class EnumerableExtensions
{
    public static IEnumerable<IList<T>> Window<T>(
        this IEnumerable<T> source, 
        int size)
    {
        if (source == null) 
            throw new ArgumentNullException(nameof(source));
        if (size <= 0) 
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive");

        return WindowImpl(source, size);
    }

    private static IEnumerable<IList<T>> WindowImpl<T>(
        IEnumerable<T> source, 
        int size)
    {
        var window = new List<T>(size);

        foreach (var item in source)
        {
            window.Add(item);

            if (window.Count == size)
            {
                yield return window.ToList(); // Return a copy
                window.RemoveAt(0); // Slide the window
            }
        }
    }
    
    public static IEnumerable<IList<T>> Batch<T>(
        this IEnumerable<T> source, 
        int batchSize)
    {
        if (source == null) 
            throw new ArgumentNullException(nameof(source));
        if (batchSize <= 0) 
            throw new ArgumentOutOfRangeException(nameof(batchSize));

        var batch = new List<T>(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        // Return final partial batch if any
        if (batch.Count > 0)
        {
            yield return batch;
        }
    }
}