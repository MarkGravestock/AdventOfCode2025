using System.Text.Unicode;
using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

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

    public class Day05Part2
    {
        private readonly ITestOutputHelper output;

        public Day05Part2(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact(Skip = "Runs out of memory")]
        
        public void it_can_count_fresh_ingredients_from_test_file()
        {
            FreshnessChecker sut = new ();
            var lines = FileReader.FromInput("day5_test.txt").AllLines().Publish();
            lines.TakeWhile(x => x.Trim() != String.Empty).ForEach(sut.AddRange);

            HashSet<long> freshIngredientIds = new HashSet<long>();
            sut.FreshIngredientsRanges().ForEach(x => x.Values.ForEach(y => freshIngredientIds.Add(y)));
            
            freshIngredientIds.Count.Should().Be(14);
        }
        
        [Fact]
        public void it_can_count_fresh_ingredients_from_file()
        {
            FreshnessChecker sut = new ();
            var lines = FileReader.FromInput("day5.txt").AllLines().Publish();
            lines.TakeWhile(x => x.Trim() != String.Empty).ForEach(sut.AddRange);

            var ranges = sut.FreshIngredientsRanges();
                ranges.Sort(((range1, range2) => range1.Start.CompareTo(range2.Start)));

            for (int i = ranges.Count - 2; i >= 0; i--)
            {
                if (ranges[i].Overlaps(ranges[i + 1]))
                {
                    ranges[i] = ranges[i].Combine(ranges[i + 1]);
                    ranges.RemoveAt(i + 1);
                }
            }
        
            ranges.ForEach(x => output.WriteLine($"{x} {x.Length}"));
            ranges.Sum(range => range.Length).Should().Be(367899984917516L);
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
    
    public bool Overlaps(Range other) => other.Start <= End;
    
    public Range Combine(Range other) => new Range(Math.Min(Start, other.Start), Math.Max(End, other.End));
    
    public long Length => End - Start + 1;
    
    public override string ToString() => $"{Start}-{End}";
    
    public IEnumerable<long> Values => Start.Through(End);
}
