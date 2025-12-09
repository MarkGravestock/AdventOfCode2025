using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day09;

public class Day09Test
{
    public class Day09Example
    {
        private readonly ITestOutputHelper output;

        public Day09Example(ITestOutputHelper output)
        {
            this.output = output;
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
        public void it_can_form_all_circuits_for_test()
        {
            var sut = new RectangleFinder("day9_test.txt", output);
        }
        

        [Fact]
        public void it_can_form_all_circuits()
        {
            var sut = new RectangleFinder("day9.txt", output);
        }
    }

}

public class RectangleFinder
{
    private readonly List<Corner> tiles;
    private readonly ITestOutputHelper output;

    private RectangleFinder(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;
        tiles = lines.Select(x => x.Split(",")).Select(y => new Corner(int.Parse(y[0]), int.Parse(y[1]))).ToList();
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

        var last = sorted.Last();
        
        return sorted.First().Area;
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
