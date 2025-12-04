using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;

namespace MarkGravestock.AdventOfCode2025.Day04;

public class Day04Test
{
    public class Day04Example
    {
        private readonly PaperRollGrid sut = new("day4_test.txt");

        [Fact]
        public void it_can_read_the_bounds_of_the_grid()
        {
            sut.Bounds().Height.Start.Value.Should().Be(0);
            sut.Bounds().Height.End.Value.Should().Be(9);
            sut.Bounds().Width.Start.Value.Should().Be(0);
            sut.Bounds().Width.End.Value.Should().Be(9);
        }

        [Fact]
        public void it_can_read_a_character_from_the_grid()
        {
            sut.ReadAt(0, 3).Should().Be("@");
        }

        [Fact]
        public void it_return_empty_when_outside_the_grid()
        {
            sut.ReadAt(-1, 3).Should().Be(".");
        }
        
        [Fact]
        public void it_find_adjacent_characters()
        {
            sut.AdjacentCountAt(0, 3).Should().Be(3);
        }
        
        [Theory]
        [InlineData(0, 3, true)]
        [InlineData(0, 7, false)]
        [InlineData(1, 3, false)]
        [InlineData(2, 6, true)]
        [InlineData(4, 9, true)]
        public void it_checks_roll_accessability(int line, int column, bool isAccessible)
        {
            sut.IsRollAccessableAt(line, column).Should().Be(isAccessible);
        }
        
        [Fact]
        public void it_finds_total_accessable_rolls()
        {
            sut.TotalAccessibleRolls().Should().Be(13);
        }
        
        [Fact]
        public void it_finds_total_accessable_rolls_after_removals()
        {
            var sut  = new IterativePaperRollGrid(FileReader.FromInput("day4_test.txt").Lines().ToList());
            
            sut.FindAccessibleRollsIteratively().Should().Be(43);
        }
    }

    public class Day04Part1
    {
        [Fact]
        public void it_finds_total_accessable_rolls()
        {
            var sut = new PaperRollGrid("day4.txt");
            sut.TotalAccessibleRolls().Should().Be(1543);
        }
    }

    public class Day04Part2
    {
        [Fact]
        public void it_finds_total_accessible_rolls_after_removals()
        {
            var sut  = new IterativePaperRollGrid(FileReader.FromInput("day4.txt").Lines().ToList());
            
            sut.FindAccessibleRollsIteratively().Should().Be(9038);
        }
    }

}

public class IterativePaperRollGrid(List<string> inputLines)
{
    public int FindAccessibleRollsIteratively()
    {
        var lines = inputLines;
        var totalRolls = 0;
        var continueLoop = true;
            
        do
        {
            var sut = new PaperRollGrid(lines);

            var results = sut.FindAccessibleRolls();

            continueLoop = results.AccessibleRollsCount > 0;

            lines = results.ResultingRolls;
            totalRolls += results.AccessibleRollsCount;
                
        } while (continueLoop);
        
        return totalRolls;
    }
}


public class PaperRollGrid(List<string> inputLines)
{
    public PaperRollGrid(string fileName) : this(FileReader.FromInput(fileName).Lines().ToList())
    {
    }

    public Bounds Bounds()
    {
        return new Bounds(new Range(0, inputLines.Count - 1), new Range(0, inputLines.First().Length - 1));
    }

    public string ReadAt(int line, int column)
    {
        if (Bounds().Height.Contains(line) && Bounds().Width.Contains(column))
        {
            return inputLines[line][column].ToString();
        }
        
        return ".";
    }

    public int AdjacentCountAt(int line, int column)
    {
        int adjacentRollCount = 0;
        
        if (ReadAt(line, column + 1) == "@") adjacentRollCount++;
        if (ReadAt(line + 1, column + 1) == "@") adjacentRollCount++;
        if (ReadAt(line + 1, column) == "@") adjacentRollCount++;
        if (ReadAt(line + 1, column - 1) == "@") adjacentRollCount++;
        if (ReadAt(line , column - 1) == "@") adjacentRollCount++;        
        if (ReadAt(line - 1, column - 1) == "@") adjacentRollCount++;
        if (ReadAt(line - 1, column) == "@") adjacentRollCount++;
        if (ReadAt(line - 1, column + 1) == "@") adjacentRollCount++;
        
        return adjacentRollCount;
    }
    
    public bool IsRollAccessableAt(int line, int column) => AdjacentCountAt(line, column) < 4;

    public int TotalAccessibleRolls()
    {
        return FindAccessibleRolls().AccessibleRollsCount;
    }

    public Results FindAccessibleRolls()
    {
        var resultingRolls = inputLines.Select(x => x).ToList();
        
        int accessibleRollCount = 0;
        
        for (int lines = Bounds().Height.Start.Value; lines <= Bounds().Height.End.Value; lines++)
        {
            for (int columms = Bounds().Width.Start.Value; columms <= Bounds().Width.End.Value; columms++)
            {
                if (ReadAt(lines, columms) == "@" && IsRollAccessableAt(lines, columms))
                {
                    accessibleRollCount++;
                    ReplaceAt(resultingRolls, lines, columms, ".");
                }
            }
        }
        
        return new Results(resultingRolls, accessibleRollCount);
    }
    
    private void ReplaceAt(List<string> resultingRolls, int lines, int columms, string replacement)
    {
        var chars = resultingRolls[lines].ToCharArray();
        chars[columms] = replacement[0];
        resultingRolls[lines] = new string(chars);
    }
}

public record Bounds(Range Height, Range Width);
public record Results(List<string> ResultingRolls, int AccessibleRollsCount);