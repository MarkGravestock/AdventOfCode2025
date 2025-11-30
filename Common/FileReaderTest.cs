using FluentAssertions;

namespace MarkGravestock.AdventOfCode2025.Common;

public class FileReaderTest
{
    [Fact]
    public void it_reads_the_lines_in_a_file()
    {
        new FileReader(@"./Input/test.txt").Lines().Should().Contain("A Y", "B X", "C Z");
    }
    
    [Fact]
    public void it_reads_the_lines_from_an_input_file()
    {
        FileReader.FromInput("test.txt").Lines().Should().HaveCount(3);
    }
}