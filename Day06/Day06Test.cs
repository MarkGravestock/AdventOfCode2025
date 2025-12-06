using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day06;

public class Day06Test
{
    public class Day06Example
    {
        private readonly Object sut = new ();

        [Fact]
        public void it_can_load_and_solve_problems()
        {
            var lines = FileReader.FromInput("day6_test.txt").AllLines().Select(x => x.Trim()).ToArray();
            
            var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            operators.Should().HaveCount(4);

            var firstOperandsLength = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
            
            firstOperandsLength.Should().Be(4);

            var operands = new long[lines.Length - 1, firstOperandsLength];
            var totals = new long[firstOperandsLength];
            
            for (int i = 0; i <= lines.Length - 2; i++)
            {
                var lineOperands = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < lineOperands.Length; j++)
                {
                    operands[i, j] = long.Parse(lineOperands[j]);

                    if (operators[j] == "+")
                    {
                        totals[j] += operands[i, j];
                    }
                    else if (operators[j] == "*")
                    {
                        if (i == 0) totals[j] = 1;
                        totals[j] *= operands[i, j];
                    }
                    else
                    {
                        throw new Exception("Unknown operator");
                    }
                }
            }
            
            totals.Sum().Should().Be(4277556);            
        }
    }

    public class Day06Part1
    {
        [Fact]
        public void it_can_load_and_solve_problems()
        {
            var lines = FileReader.FromInput("day6.txt").AllLines().Select(x => x.Trim()).ToArray();
            
            var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            operators.Should().HaveCount(1000);

            var firstOperandsLength = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
            
            firstOperandsLength.Should().Be(1000);

            var operands = new long[lines.Length - 1, firstOperandsLength];
            var totals = new long[firstOperandsLength];
            
            for (int i = 0; i <= lines.Length - 2; i++)
            {
                var lineOperands = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < lineOperands.Length; j++)
                {
                    operands[i, j] = long.Parse(lineOperands[j]);

                    if (operators[j] == "+")
                    {
                        totals[j] += operands[i, j];
                    }
                    else if (operators[j] == "*")
                    {
                        if (i == 0) totals[j] = 1;
                        totals[j] *= operands[i, j];
                    }
                    else
                    {
                        throw new Exception("Unknown operator");
                    }
                }
            }
            
            totals.Sum().Should().Be(5361735137219L);
        }
    }

    public class Day06Part2
    {
        private readonly ITestOutputHelper output;

        public Day06Part2(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public void it_can_load_and_solve_example_problems()
        {
            var lines = FileReader.FromInput("day6_test.txt").AllLines().Select(x => x.Trim()).ToArray();
            
            var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            operators.Should().HaveCount(4);

            var firstOperandsLength = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
            
            firstOperandsLength.Should().Be(4);

            var operands = new long[lines.Length - 1, firstOperandsLength];
            var totals = new long[firstOperandsLength];
            
            for (int i = 0; i <= lines.Length - 2; i++)
            {
                var lineOperands = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < lineOperands.Length; j++)
                {
                    operands[i, j] = long.Parse(lineOperands[j]);

                    if (operators[j] == "+")
                    {
                        totals[j] += operands[i, j];
                    }
                    else if (operators[j] == "*")
                    {
                        if (i == 0) totals[j] = 1;
                        totals[j] *= operands[i, j];
                    }
                    else
                    {
                        throw new Exception("Unknown operator");
                    }
                }
            }
        }
    }
}

