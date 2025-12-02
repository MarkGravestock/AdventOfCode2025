using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;

namespace MarkGravestock.AdventOfCode2025.Day02;

public class Day02Test
{
    public class Day02Example
    {
        [Fact]
        public void it_can_split_a_range()
        {
            var range = new ProductRange("11-22");
            range.FirstId.Should().Be(11);
            range.LastId.Should().Be(22);
        }
        
        [Fact]
        public void it_can_find_invalid_single_values()
        {
            var range = new ProductRange("11-22");
            range.IsInvalid(22).Should().Be(true);
            range.IsInvalid(15).Should().Be(false);
        }

        [Theory]
        [InlineData("11-22", new[]{11L, 22L})]
        [InlineData("95-115", new[]{99L})]
        [InlineData("1188511880-1188511890", new[]{1188511885L})]
        [InlineData("1698522-1698528", new long[]{})]
        public void it_can_find_all_invalid_values(string productIdRange, long[] invalidIds)
        {
            var range = new ProductRange(productIdRange);
            range.InvalidIds.Should().Equal(invalidIds);
        }
        
        [Fact]
        public void it_can_find_the_total_of_invalid_ids()
        {
            var lines = FileReader.FromInput("day2_test.txt").Lines().Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).SelectMany(x => x);
            var checksum = lines.Sum(line => new ProductRange(line).InvalidIds.Sum());
            checksum.Should().Be(1227775554);
        }
    }

    public class Day02Part1
    {
        [Fact]
        public void it_can_find_the_total_of_invalid_ids()
        {
            var lines = FileReader.FromInput("day2.txt").Lines()
                .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).SelectMany(x => x);
            long checksum = lines.Sum(line => new ProductRange(line).InvalidIds.Sum(x => (long)x));
            checksum.Should().Be(40214376723L);
        }
    }
}

public class ProductRange
{
    public ProductRange(string productIdRange)
    {
        var items = productIdRange.Split('-');

        if (items[0].StartsWith('0')) throw new ArgumentException("Invalid range", nameof(productIdRange));
        
        FirstId = long.Parse(items[0]);
        LastId = long.Parse(items[1]);
    }

    public List<long> InvalidIds => FirstId.Through(LastId).Where(IsInvalid).ToList();
    public long FirstId { get; }
    public long LastId { get; }

    public bool IsInvalid(long id)
    {
        var idString = id.ToString();
        var idLength = idString.Length;
        
        if (idLength.IsOdd()) return false;

        var first = idString[..(idLength / 2)];
        var second = idString[(idLength / 2)..];
        return first == second;
    }
}