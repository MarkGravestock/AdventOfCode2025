using FluentAssertions;
using FluentAssertions.Formatting;
using MarkGravestock.AdventOfCode2025.Common;

namespace MarkGravestock.AdventOfCode2025.Day05;

public class Day05Test
{
    public class Day05Example
    {
        private readonly FreshnessChecker sut = new ();

        [Fact]
        public void it_can_add_ranges()
        {
            sut.AddRange("3-5");
            sut.FreshIngredientsRanges().Should().HaveCount(1);
        }
        
        [Fact]
        public void it_can_add_check_if_fresh()
        {
            sut.AddRange("3-5");
            sut.IsFresh("5").Should().BeTrue();
            sut.IsFresh("1").Should().BeFalse();
        }

        [Fact]
        public void it_can_check_add_ranges_from_file()
        {
            var lines = FileReader.FromInput("day5_test.txt").AllLines().Publish();
            lines.TakeWhile(x => x.Trim() != String.Empty).ForEach(sut.AddRange);
            sut.FreshIngredientsRanges().Should().HaveCount(4);
            lines.Sum(x => sut.IsFresh(x) ? 1 : 0 ).Should().Be(4);
        }
    }

    public class Day05Part1
    {
        [Fact]
        public void it_can_check_add_ranges_from_file()
        {
            FreshnessChecker sut = new ();
            var lines = FileReader.FromInput("day5.txt").AllLines().Publish();
            lines.TakeWhile(x => x.Trim() != String.Empty).ForEach(sut.AddRange);
            lines.Sum(x => sut.IsFresh(x) ? 1 : 0 ).Should().Be(601);
        }
    }

    public class Day04Part2
    {
        [Fact]
        public void it_works()
        {
            "sut".Should().Be("sut");
        }
    }
}

internal class FreshnessChecker
{
    private readonly List<Range> ranges = new();
    
    public void AddRange(string rangeDefinition)
    {
        var items = rangeDefinition.Split('-');
        ranges.Add(new Range(long.Parse(items[0]), long.Parse(items[1])));
    }

    public List<Range> FreshIngredientsRanges()
    {
        return ranges;
    }

    public bool IsFresh(string ingredientId)
    {
        var ingredientIdInt = long.Parse(ingredientId);
        return ranges.Any(x => x.Contains(ingredientIdInt));
    }
}

internal record Range(long Start, long End)
{
    public bool Contains(long value) => Start <= value && value <= End;
}
