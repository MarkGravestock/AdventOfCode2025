namespace MarkGravestock.AdventOfCode2025.Common;

public class Grid(List<string> inputLines, GridOptions options)
{
    private GridOptions Options { get; } = options;

    public Grid(List<string> inputLines) : this(inputLines, GridOptions.Default())
    {
    }
    
    public Bounds Bounds()
    {
        return new Bounds(new Range(0, inputLines.Count - 1), new Range(0, inputLines.First().Length - 1));
    }

    public string ReadAt(Location location)
    {
        if (Bounds().Contains(location))
        {
            return inputLines[location.Line.Value][location.Column.Value].ToString();
        }
        
        return Options.OutOfBounds.Invoke();
    }
}

public class GridOptions
{
    public static GridOptions Default()
    {
        return new GridOptions();
    }
    
    public readonly Func<string> OutOfBounds = () => "X";
}

public record Bounds(Range Height, Range Width)
{
    public bool Contains(Location location)
    {
        return Height.Contains(location.Line.Value) && Width.Contains(location.Column.Value);
    }
}

public record Location(Index Line, Index Column)
{
    internal Location MoveDown()
    {
        return this with { Line = Line.Value + 1 };
    }
}