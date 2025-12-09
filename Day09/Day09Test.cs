using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using NetTopologySuite.Geometries;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day09;

public class Day09Test
{
    public class Day09Example
    {
        public Day09Example(ITestOutputHelper output)
        {
            sut = new RectangleFinder("day9_test.txt", output);
        }

        private readonly RectangleFinder sut;

        [Fact] public void it_can_calculate_area()
        {
            new Rectangle(new Corner(7,3), new Corner(2, 3)).Area.Should().Be(6);
            new Rectangle(new Corner(2,5), new Corner(9, 7)).Area.Should().Be(24);
            new Rectangle(new Corner(7,1), new Corner(11, 7)).Area.Should().Be(35);
            new Rectangle(new Corner(11, 7), new Corner(7,1)).Area.Should().Be(35);
            new Rectangle(new Corner(7, 11), new Corner(1,7)).Area.Should().Be(35);
        }

        [Fact]
        public void it_can_calculate_another_area()
        {
            new Rectangle(new Corner (59305, 97788), new Corner (96927, 40706)).Area.Should().Be(2147633709);
        }

        [Fact]
        public void it_can_calculate_largest_area()
        {
            sut.FindLargestRectangleArea().Should().Be(50);
        }
    }

    public class Day09Part1(ITestOutputHelper output)
    {
        private readonly RectangleFinder sut = new("day9.txt", output);

        [Fact]
        public void it_can_calculate_largest_area()
        {
            sut.FindLargestRectangleArea().Should().Be(4738108384);
        }
    }

    public class Day09Part2(ITestOutputHelper output)
    {
        
        [Fact]
        public void it_can_check_rectangle_is_in_bounds()
        {
            var sut = new RectangleFinder("day9_test.txt", output);

            new Rectangle(new Corner(2, 5), new Corner(11,1)).Area.Should().Be(50);
            sut.IsInBounds(new Rectangle(new Corner(2, 5), new Corner(11,1))).Should().Be(false);
            sut.IsInBounds(new Rectangle(new Corner(7, 3), new Corner(11,1))).Should().Be(true);
        }
        
        [Fact]
        public void it_can_find_largest_rectangle_in_bound_for_example()
        {
            var sut = new RectangleFinder("day9_test.txt", output);
            sut.FindLargestRectangleInBoundArea().Should().Be(24);
        }

        
        [Fact]
        public void it_can_find_largest_rectangle_in_bound()
        {
            var sut = new RectangleFinder("day9.txt", output);
            sut.FindLargestRectangleInBoundArea().Should().Be(1513792010);
        }
    }

}

public class RectangleFinder
{
    private readonly List<Corner> tiles;
    private readonly ITestOutputHelper output;
    private readonly GeometryFactory factory = new();
    private readonly Polygon boundary;

    private RectangleFinder(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;
        tiles = lines.Select(x => x.Split(",")).Select(y => new Corner(int.Parse(y[0]), int.Parse(y[1]))).ToList();
        
        var coordinates = tiles.Select(x => new Coordinate(x.X.Value, x.Y.Value)).ToArray();
        coordinates = coordinates.Append(coordinates[0]).ToArray();
        
        boundary = factory.CreatePolygon(coordinates);
    }
    
    public RectangleFinder(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
    }

    public double FindLargestRectangleArea()
    {
        var allCombinations = tiles.SelectMany((first, i) => 
                tiles.Skip(i + 1).Select(second => new Rectangle(first, second)))
            .ToList();
        
        var sorted = allCombinations.OrderByDescending(x => x.Area);
        
        return sorted.First().Area;
    }


    public bool IsInBounds(Rectangle rectangle)
    {

        var x1 = rectangle.Corner1.X.Value;
        var x2 = rectangle.Corner2.X.Value;
        var y1 = rectangle.Corner1.Y.Value;
        var y2 = rectangle.Corner2.Y.Value;
 
        Coordinate[] rectCoords = {
            new(x1, y1),
            new(x2, y1),
            new(x2, y2),
            new(x1, y2),
            new(x1, y1)
        };

        Polygon rectangleShape = factory.CreatePolygon(rectCoords);
        
        return boundary.Covers(rectangleShape);
    }

    public object FindLargestRectangleInBoundArea()
    {
        var allCombinations = tiles.SelectMany((first, i) =>
                tiles.Skip(i + 1).Select(second => new Rectangle(first, second)))
            .ToList();

        var largestToSmallest = allCombinations.OrderByDescending(x => x.Area);
        
        var largestThatFits =  largestToSmallest.First(x => IsInBounds(x));
        
        GenerateSvg(new []{largestThatFits}.ToList());

        return largestThatFits.Area;
    }

    private void GenerateSvg(List<Rectangle> inBounds)
    {
        // Calculate bounding box with padding
        var minX = tiles.Min(t => t.X.Value);
        var maxX = tiles.Max(t => t.X.Value);
        var minY = tiles.Min(t => t.Y.Value);
        var maxY = tiles.Max(t => t.Y.Value);

        var width = maxX - minX;
        var height = maxY - minY;
        var padding = Math.Max(width, height) * 0.05;

        var viewBoxMinX = minX - padding;
        var viewBoxMinY = minY - padding;
        var viewBoxWidth = width + (2 * padding);
        var viewBoxHeight = height + (2 * padding);

        // Start SVG
        var svg = new System.Text.StringBuilder();
        svg.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"{viewBoxMinX} {viewBoxMinY} {viewBoxWidth} {viewBoxHeight}\">");

        // Calculate gradient parameters
        var minArea = inBounds.Min(r => r.Area);
        var maxArea = inBounds.Max(r => r.Area);
        var strokeWidth = Math.Min(viewBoxWidth, viewBoxHeight) * 0.003;
        var boundaryStrokeWidth = Math.Min(viewBoxWidth, viewBoxHeight) * 0.015;

        // Render rectangles (smallest to largest)
        foreach (var rect in inBounds.OrderBy(r => r.Area))
        {
            var rectMinX = Math.Min(rect.Corner1.X.Value, rect.Corner2.X.Value);
            var rectMinY = Math.Min(rect.Corner1.Y.Value, rect.Corner2.Y.Value);
            var rectWidth = Math.Abs(rect.Corner1.X.Value - rect.Corner2.X.Value) + 1;
            var rectHeight = Math.Abs(rect.Corner1.Y.Value - rect.Corner2.Y.Value) + 1;

            // Calculate color based on area
            var normalized = (double)(rect.Area - minArea) / (maxArea - minArea);
            var lightness = 80 - (normalized * 50);
            var color = $"hsl(120, 70%, {lightness:F1}%)";

            svg.AppendLine($"  <rect x=\"{rectMinX}\" y=\"{rectMinY}\" width=\"{rectWidth}\" height=\"{rectHeight}\" " +
                          $"fill=\"{color}\" fill-opacity=\"0.7\" stroke=\"hsl(120, 70%, 20%)\" stroke-width=\"{strokeWidth}\"/>");
        }

        // Render boundary polygon
        var coordinates = boundary.Coordinates;
        var points = string.Join(" ", coordinates.Select(c => $"{c.X},{c.Y}"));
        svg.AppendLine($"  <polygon points=\"{points}\" fill=\"none\" stroke=\"black\" stroke-width=\"{strokeWidth}\" stroke-linejoin=\"round\"/>");

        svg.AppendLine("</svg>");

        // Write to file
        var filename = "day9-visualization.svg";
        File.WriteAllText($"{filename}", svg.ToString());
        output.WriteLine($"SVG written to {Path.GetFullPath(filename)}");
    }
}

public record Rectangle(Corner Corner1, Corner Corner2)
{
    public bool Same(Rectangle other) => Corner1.Equals(other.Corner1) && Corner1.Equals(other.Corner1) || Corner1.Equals(other.Corner2) && Corner2.Equals(other.Corner1);

    public long Area =>  Width * Height;

    public long Height => (Math.Abs(Corner1.Y.Value - Corner2.Y.Value) + 1);
    public long Width => (Math.Abs(Corner1.X.Value - Corner2.X.Value) + 1);
}

public record Corner(Index X, Index Y);
