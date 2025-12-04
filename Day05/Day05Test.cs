using FluentAssertions;

namespace MarkGravestock.AdventOfCode2025.Day05;

public class Day05Test
{
    public class Day05Example
    {
        private readonly string sut = "sut";

        [Fact]
        public void it_works()
        {
            sut.Should().Be("sut");
        }
    }

    public class Day04Part1
    {
        [Fact]
        public void it_works()
        {
            "sut".Should().Be("sut");
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
