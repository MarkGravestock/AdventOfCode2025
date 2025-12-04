namespace MarkGravestock.AdventOfCode2025.Common;

public static class RangeExtensions
{
    public static IEnumerable<int> Through(this int start, int end)
    {
        return Enumerable.Range(start, end - start + 1);
    }
    
    public static IEnumerable<long> Through(this long start, long end)
    {
        for (long i = start; i <= end; i++)
        {
            yield return i;
        }
    }

    public static bool Contains(this Range range, int value)
    {
        return range.Start.Value <= value && value <= range.End.Value;
    }
}