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
    private Polygon boundary;

    public RectangleFinder(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
    }

    private RectangleFinder(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;
        tiles = lines.Select(x => x.Split(",")).Select(y => new Corner(int.Parse(y[0]), int.Parse(y[1]))).ToList();
        
        boundary = CreateBoundary();
    }

    private Polygon CreateBoundary()
    {
        var coordinates = tiles.Select(x => new Coordinate(x.X.Value, x.Y.Value)).ToArray();
        coordinates = coordinates.Append(coordinates[0]).ToArray();
        return factory.CreatePolygon(coordinates);
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
 
        Coordinate[] rectCoords =
        [
            new(x1, y1),
            new(x2, y1),
            new(x2, y2),
            new(x1, y2),
            new(x1, y1)
        ];

        Polygon rectangleShape = factory.CreatePolygon(rectCoords);
        
        return boundary.Covers(rectangleShape);
    }

    public object FindLargestRectangleInBoundArea()
    {
        var allCombinations = tiles.SelectMany((first, i) =>
                tiles.Skip(i + 1).Select(second => new Rectangle(first, second)))
            .ToList();

        var largestToSmallest = allCombinations.OrderByDescending(x => x.Area);
        
        var largestThatFits =  largestToSmallest.First(IsInBounds);
        
        GenerateSvg(new []{largestThatFits}.ToList());

        return largestThatFits.Area;
    }

    private void GenerateSvg(List<Rectangle> rectangleToRender)
    {
        var filename = "day9-visualization.svg";
        var outputPath = $"../../../Output/{filename}";
        var generator = new SvgGenerator(tiles, boundary, outputPath);
        var fullPath = generator.GenerateVisualization(rectangleToRender);
        output.WriteLine($"SVG written to {fullPath}");
    }
}

public record Rectangle(Corner Corner1, Corner Corner2)
{
    
    
    public long Area =>  Width * Height;

    public long Height => (Math.Abs(Corner1.Y.Value - Corner2.Y.Value) + 1);
    public long Width => (Math.Abs(Corner1.X.Value - Corner2.X.Value) + 1);
}

public record Corner(Index X, Index Y);
