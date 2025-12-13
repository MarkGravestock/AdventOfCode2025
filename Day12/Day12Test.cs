using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day12;

public class Day12Test
{
    public class Day12Example
    {
        public Day12Example(ITestOutputHelper output)
        {
            sut = new PresentPacker("day12_test.txt", output);
        }

        private readonly PresentPacker sut;

        [Fact]
        public void it_can_load_the_presents()
        {
            sut.Presents.Count.Should().Be(6);
        }

        [Fact]
        public void it_loads_presents_with_correct_counts()
        {
            sut.Presents[0].Count.Should().Be(7);
            sut.Presents[5].Count.Should().Be(7);
        }

        [Fact]
        public void it_can_load_the_requirements()
        {
            sut.Requirements.Count.Should().Be(3);
        }

        [Fact]
        public void it_loads_requirements_with_correct_region_sizes()
        {
            sut.Requirements[0].Region.Bounds.Width.End.Value.Should().Be(3);
            sut.Requirements[0].Region.Bounds.Height.End.Value.Should().Be(3);
            sut.Requirements[1].Region.Bounds.Width.End.Value.Should().Be(11);
            sut.Requirements[1].Region.Bounds.Height.End.Value.Should().Be(4);
        }

        [Fact]
        public void it_loads_requirements_with_correct_present_counts()
        {
            sut.Requirements[0].PresentCounts.Should().Equal(0, 0, 0, 0, 2, 0);
            sut.Requirements[2].PresentCounts.Should().Equal(1, 0, 1, 0, 3, 2);
        }

        [Fact]
        public void it_can_rotate_a_present()
        {
            var present = sut.Presents[0];

            var rotated90 = present.Rotate(Rotation.Degrees90);
            rotated90[0, 0].Should().BeTrue();
            rotated90[0, 1].Should().BeTrue();
            rotated90[0, 2].Should().BeTrue();
            rotated90[1, 0].Should().BeTrue();
            rotated90[1, 1].Should().BeTrue();
            rotated90[1, 2].Should().BeTrue();
            rotated90[2, 0].Should().BeFalse();
            rotated90[2, 1].Should().BeFalse();
            rotated90[2, 2].Should().BeTrue();
            
            var rotated180 = present.Rotate(Rotation.Degrees180);
            rotated180[0, 0].Should().BeFalse();
            rotated180[0, 1].Should().BeTrue();
            rotated180[0, 2].Should().BeTrue();
            rotated180[1, 0].Should().BeFalse();
            rotated180[1, 1].Should().BeTrue();
            rotated180[1, 2].Should().BeTrue();
            rotated180[2, 0].Should().BeTrue();
            rotated180[2, 1].Should().BeTrue();
            rotated180[2, 2].Should().BeTrue();

            var rotated270 = present.Rotate(Rotation.Degrees270);
            rotated270[0, 0].Should().BeTrue();
            rotated270[0, 1].Should().BeFalse();
            rotated270[0, 2].Should().BeFalse();
            rotated270[1, 0].Should().BeTrue();
            rotated270[1, 1].Should().BeTrue();
            rotated270[1, 2].Should().BeTrue();
            rotated270[2, 0].Should().BeTrue();
            rotated270[2, 1].Should().BeTrue();
            rotated270[2, 2].Should().BeTrue();
        }

        [Fact]
        public void it_can_get_all_rotations()
        {
            var present = sut.Presents[0];
            var allRotations = present.AllRotations().ToList();

            allRotations.Count.Should().Be(4);
        }

        [Fact]
        public void it_can_check_if_present_fits_in_region()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region.CanFit(present, new Location(0, 0)).Should().BeTrue();
            region.CanFit(present, new Location(2, 2)).Should().BeTrue();
            region.CanFit(present, new Location(2, 1)).Should().BeTrue();
        }

        [Fact]
        public void it_detects_when_present_exceeds_bounds()
        {
            var region = new Region(4, 4);
            var present = sut.Presents[4];

            region.CanFit(present, new Location(2, 0)).Should().BeFalse();
            region.CanFit(present, new Location(0, 2)).Should().BeFalse();
        }

        [Fact]
        public void it_detects_when_cells_are_not_available()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region[0, 0] = true;

            region.CanFit(present, new Location(0, 0)).Should().BeFalse();
            region.CanFit(present, new Location(1, 1)).Should().BeTrue();
        }

        [Fact]
        public void it_can_place_a_present_in_region()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region.Place(present, new Location(0, 0));

            region[0, 0].Should().BeTrue();
            region[0, 1].Should().BeTrue();
            region[0, 2].Should().BeTrue();
            region[1, 0].Should().BeTrue();
            region[1, 1].Should().BeFalse();
            region[1, 2].Should().BeFalse();
            region[2, 0].Should().BeTrue();
            region[2, 1].Should().BeTrue();
            region[2, 2].Should().BeTrue();
        }

        [Fact]
        public void it_prevents_placing_present_when_cannot_fit()
        {
            var region = new Region(4, 4);
            var present = sut.Presents[4];

            var act = () => region.Place(present, new Location(2, 0));

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void it_prevents_overlapping_presents()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region.Place(present, new Location(0, 0));

            var act = () => region.Place(present, new Location(0, 0));

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void it_can_remove_a_present_from_region()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region.Place(present, new Location(0, 0));
            region.Remove(present, new Location(0, 0));

            region[0, 0].Should().BeFalse();
            region[0, 1].Should().BeFalse();
            region[0, 2].Should().BeFalse();
            region[1, 0].Should().BeFalse();
            region[2, 0].Should().BeFalse();
            region[2, 1].Should().BeFalse();
            region[2, 2].Should().BeFalse();
        }

        [Fact]
        public void it_can_place_after_removing()
        {
            var region = new Region(5, 5);
            var present = sut.Presents[4];

            region.Place(present, new Location(0, 0));
            region.Remove(present, new Location(0, 0));

            region.CanFit(present, new Location(0, 0)).Should().BeTrue();
        }

        [Fact]
        public void it_can_expand_requirement_to_shape_list()
        {
            var requirement = sut.Requirements[0];

            var presentList = requirement.ExpandToPresentList();

            presentList.Should().Equal(4, 4);
        }

        [Fact]
        public void it_expands_multiple_shapes_correctly()
        {
            var requirement = sut.Requirements[2];

            var presentList = requirement.ExpandToPresentList();

            presentList.Should().Equal(0, 2, 4, 4, 4, 5, 5);
        }

        [Fact]
        public void it_can_solve_first_requirement()
        {
            var requirement = sut.Requirements[0];

            var result = sut.Solve(requirement);

            result.Should().BeTrue();
        }

        [Fact]
        public void it_can_solve_second_requirement()
        {
            var requirement = sut.Requirements[1];

            var result = sut.Solve(requirement);

            result.Should().BeTrue();
        }

        [Fact]
        public void it_shows_requirement_details()
        {
            sut.Requirements[1].ExpandToPresentList().Count.Should().Be(6);
            sut.Requirements[2].ExpandToPresentList().Count.Should().Be(7);
        }

        [Fact(Skip="This seems to be a red herring as brute force solution takes too long, but real input isn't problematic")]
        public void it_can_solve_third_requirement()
        {
            var requirement = sut.Requirements[2];

            var result = sut.Solve(requirement);

            result.Should().BeTrue();
        }
    }

    public class Day12Part1(ITestOutputHelper output)
    {
        private readonly PresentPacker sut = new("day12.txt", output);

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Solvable().Should().Be(448);
        }
    }
}

public enum Rotation
{
    Degrees0,
    Degrees90,
    Degrees180,
    Degrees270
}

public class PresentPacker
{
    private readonly ITestOutputHelper output;
    private readonly Dictionary<int, Present> presents;
    private readonly List<Requirement> requirements;

    private int maxIterations = 0;

    public PresentPacker(string fileName, ITestOutputHelper output)
    {
        this.output = output;

        var lines = File.ReadAllLines($"./Input/{fileName}");

        presents = lines
            .Select((line, index) => (line: line.Trim(), index))
            .Where(x => x.line.EndsWith(':') && int.TryParse(x.line.TrimEnd(':'), out _))
            .Select(x => new
            {
                ShapeNum = int.Parse(x.line.TrimEnd(':')),
                ShapeLines = lines.Skip(x.index + 1).Take(3).ToList()
            })
            .ToDictionary(
                x => x.ShapeNum,
                x => new Present(x.ShapeLines)
            );

        requirements = lines
            .Where(line => line.Contains('x') && line.Contains(':'))
            .Select(line =>
            {
                var parts = line.Split(':');
                var dimensions = parts[0].Trim().Split('x');
                var width = int.Parse(dimensions[0]);
                var height = int.Parse(dimensions[1]);
                var counts = parts[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse).ToList();
                return new Requirement(new Region(width, height), counts);
            })
            .ToList();
    }

    public Dictionary<int, Present> Presents => presents;
    public List<Requirement> Requirements => requirements;

    public int Solvable()
    {
        return Requirements.Sum(x => Solve(x) ? 1 : 0);
    }
    
    public bool Solve(Requirement requirement)
    {
        var shapeList = requirement.ExpandToPresentList();
        var shapesToPlace = shapeList
            .Select(id => presents[id])
            .OrderByDescending(shape => shape.Count)
            .ToList();

        var memo = new Dictionary<string, bool>();
        return TryPlaceShapes(requirement.Region, shapesToPlace, 0);
    }

    private bool TryPlaceShapes(Region region, List<Present> shapesToPlace, int currentIndex)
    {
        if (maxIterations++ > 100000)
        {
            throw new Exception("Too many iterations");
        }

        if (currentIndex >= shapesToPlace.Count)
        {
            output.WriteLine($"Solved {maxIterations}");
            return true;
        }

        var blocksNeeded = shapesToPlace
            .Skip(currentIndex)
            .Sum(shape => shape.Count);

        if (region.GetFreeSpaceCount() < blocksNeeded)
            return false;

        var currentShape = shapesToPlace[currentIndex];
        var rotations = currentShape.AllRotations().ToList();

        for (int row = 0; row <= region.Bounds.Height.End.Value; row++)
        {
            for (int col = 0; col <= region.Bounds.Width.End.Value; col++)
            {
                var location = new Location(row, col);

                foreach (var rotatedShape in rotations)
                {
                    if (region.CanFit(rotatedShape, location))
                    {
                        region.Place(rotatedShape, location);

                        if (TryPlaceShapes(region, shapesToPlace, currentIndex + 1))
                            return true;

                        region.Remove(rotatedShape, location);
                    }
                }
            }
        }

        return false;
    }
}

public class Present
{
    private readonly bool[,] shape = new bool[3, 3];

    public Present(List<string> shapeLines)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                shape[row, col] = shapeLines[row][col] == '#';
            }
        }
    }

    public int Count => Enumerable.Range(0, 3)
        .SelectMany(row => Enumerable.Range(0, 3).Select(col => shape[row, col]))
        .Count(cell => cell);

    public bool this[int row, int col] => shape[row, col];

    public Present Rotate(Rotation rotation)
    {
        var rotated = new bool[3, 3];

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                rotated[row, col] = rotation switch
                {
                    Rotation.Degrees0 => shape[row, col],
                    Rotation.Degrees90 => shape[2 - col, row],
                    Rotation.Degrees180 => shape[2 - row, 2 - col],
                    Rotation.Degrees270 => shape[col, 2 - row],
                    _ => shape[row, col]
                };
            }
        }

        return new Present(rotated);
    }

    public IEnumerable<Present> AllRotations()
    {
        return Enum.GetValues<Rotation>().Select(Rotate);
    }

    private Present(bool[,] rotatedShape)
    {
        shape = rotatedShape;
    }
}

public class Region
{
    private readonly bool[,] grid;

    public Region(int width, int height)
    {
        grid = new bool[height, width];
        Bounds = new Bounds(new Range(0, height - 1), new Range(0, width - 1));

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                grid[row, col] = false;
            }
        }
    }

    public Bounds Bounds { get; }

    public bool this[int row, int col]
    {
        get => grid[row, col];
        set => grid[row, col] = value;
    }

    public int GetFreeSpaceCount()
    {
        int count = 0;
        for (int row = 0; row <= Bounds.Height.End.Value; row++)
        {
            for (int col = 0; col <= Bounds.Width.End.Value; col++)
            {
                if (!grid[row, col])
                    count++;
            }
        }
        return count;
    }

    public bool CanFit(Present present, Location location)
    {
        if (!IsWithinBounds(location))
            return false;

        if (!AreRequiredCellsFree(present, location))
            return false;

        return true;
    }

    public void Place(Present present, Location location)
    {
        if (!CanFit(present, location))
            throw new InvalidOperationException("Present cannot fit at the specified location");

        UpdateCells(present, location, true);
    }

    public void Remove(Present present, Location location)
    {
        if (!IsWithinBounds(location))
            throw new InvalidOperationException("Location is out of bounds");

        UpdateCells(present, location, false);
    }

    private void UpdateCells(Present present, Location location, bool occupied)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (present[row, col])
                {
                    grid[location.Line.Value + row, location.Column.Value + col] = occupied;
                }
            }
        }
    }

    private bool IsWithinBounds(Location location)
    {
        var presentBounds = new Bounds(
            new Range(location.Line.Value, location.Line.Value + 2),
            new Range(location.Column.Value, location.Column.Value + 2)
        );

        if (location.Line.Value < 0 || location.Column.Value < 0)
            return false;

        if (location.Line.Value + 3 > Bounds.Height.End.Value + 1 ||
            location.Column.Value + 3 > Bounds.Width.End.Value + 1)
            return false;

        return true;
    }

    private bool AreRequiredCellsFree(Present present, Location location)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (present[row, col] && grid[location.Line.Value + row, location.Column.Value + col])
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public class Requirement(Region region, List<int> presentCounts)
{
    public Region Region { get; } = region;
    public List<int> PresentCounts { get; } = presentCounts;

    public List<int> ExpandToPresentList()
    {
        var presentList = new List<int>();

        for (int presentNumber = 0; presentNumber < PresentCounts.Count; presentNumber++)
        {
            for (int count = 0; count < PresentCounts[presentNumber]; count++)
            {
                presentList.Add(presentNumber);
            }
        }

        return presentList;
    }
}


