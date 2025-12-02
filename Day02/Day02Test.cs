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
    
    public class Day02Part2
    {
        [Fact]
        public void it_can_find_invalid_single_values()
        {
            var range = new ProductRangePart2("998-1012");
            range.IsInvalid(999).Should().Be(true);
            range.IsInvalid(1000).Should().Be(false);
            range.IsInvalid(1010).Should().Be(true);
        }
        
        [Theory]
        [InlineData("11-22", new[]{11L, 22L})]
        [InlineData("95-115", new[]{99L, 111L})]
        [InlineData("1188511880-1188511890", new[]{1188511885L})]
        [InlineData("1698522-1698528", new long[]{})]
        [InlineData("222220-222224", new long[]{222222})]
        [InlineData("565653-565659", new long[]{565656})]
        [InlineData("824824821-824824827 ", new long[]{824824824})]
        [InlineData("2121212118-2121212124", new long[]{2121212121L})]
        public void it_can_find_all_invalid_values(string productIdRange, long[] invalidIds)
        {
            var range = new ProductRangePart2(productIdRange);
            range.InvalidIds.Should().Equal(invalidIds);
        }

        [Fact]
        public void it_can_find_the_total_of_the_example_invalid_ids()
        {
            var lines = FileReader.FromInput("day2_test.txt").Lines().Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).SelectMany(x => x);
            var checksum = lines.Sum(line => new ProductRangePart2(line).InvalidIds.Sum());
            checksum.Should().Be(4174379265L);
        }
        
        [Fact]
        public void it_can_find_the_total_of_invalid_ids()
        {
            var lines = FileReader.FromInput("day2.txt").Lines()
                .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()).SelectMany(x => x);
            long checksum = lines.Sum(line => new ProductRangePart2(line).InvalidIds.Sum(x => (long)x));
            checksum.Should().Be(50793864718L);
        }
    }
}

public class ProductRangePart2 : ProductRangeBase
{
    public ProductRangePart2(string productIdRange) : base(productIdRange)
    {
    }

    public override bool IsInvalid(long id)
    {
        var idString = id.ToString();
        var positions = 1.Through(idString.Length / 2);

        foreach (var position in positions)
        {
            var repeated = Enumerable.Repeat(idString[..position], idString.Length / position);
            var match = string.Concat(repeated);
            if (match == idString) return true;
        }
        
        return false;
    }
}

public abstract class ProductRangeBase
{
    public ProductRangeBase(string productIdRange)
    {
        var items = productIdRange.Split('-');

        if (items[0].StartsWith('0')) throw new ArgumentException("Invalid range", nameof(productIdRange));
        
        FirstId = long.Parse(items[0]);
        LastId = long.Parse(items[1]);
    }

    public List<long> InvalidIds => RangeExtensions.Through((long)FirstId, LastId).Where(IsInvalid).ToList();
    public long FirstId { get; }
    public long LastId { get; }
    public abstract bool IsInvalid(long id);
}

public class ProductRange : ProductRangeBase
{
    public ProductRange(string productIdRange) : base(productIdRange)
    {
    }

    public override bool IsInvalid(long id)
    {
        var idString = id.ToString();
        var idLength = idString.Length;
        
        if (idLength.IsOdd()) return false;

        var first = idString[..(idLength / 2)];
        var second = idString[(idLength / 2)..];
        return first == second;
    }
}