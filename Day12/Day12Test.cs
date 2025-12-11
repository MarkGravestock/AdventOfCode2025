using FluentAssertions;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day12;

public class Day12Test
{
    public class Day12Example
    {
        public Day12Example(ITestOutputHelper output)
        {
            sut = new Solution("day12_test.txt", output);
        }

        private readonly Solution sut;
        

        [Fact]
        public void it_can_read_the_devices()
        {
            sut.Should().NotBe(null);
        }

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Should().NotBe(null);
        }
    }

    public class Day11Part1(ITestOutputHelper output)
    {
        private readonly Solution sut = new("day12.txt", output);

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Should().NotBe(null);
        }
    }

    public class Day11Part2(ITestOutputHelper output)
    {
        [Fact]
        public void it_can_count_the_machines_in_examples()
        {
            var sut = new Solution("day12_test2.txt", output);
            sut.Should().NotBe(null);
        }

        
        [Fact]
        public void it_can_count_the_machines()
        {
            var sut = new Solution("day12.txt", output);
            sut.Should().NotBe(null);
        }
    }

}

internal class Solution(string fileName, ITestOutputHelper output)
{
}

