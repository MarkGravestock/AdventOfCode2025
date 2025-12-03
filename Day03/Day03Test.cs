using System.Text;
using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;

namespace MarkGravestock.AdventOfCode2025.Day03;

public class Day03Test
{
    public class Day03Example
    {
        [Theory]
        [InlineData("987654321111111", 9, 8)]
        [InlineData("811111111111119", 8, 9)] 
        [InlineData("818181911112111", 9, 2)]
        [InlineData("234234234234278", 7, 8)] 
        public void it_can_find_the_joltages(string batteries, int firstJoltage, int secondJoltage)
        {
            var range = new TwoBatteryBank(batteries);
            range.FirstJoltage.Should().Be(firstJoltage);
            range.SecondJoltage.Should().Be(secondJoltage);
            range.TotalJoltage().Should().Be(firstJoltage * 10 + secondJoltage);
        }
        
        [Fact]
        public void it_can_find_the_total_test_joltage()
        {
            var sut = new JoltageMaximiser("day3_test.txt");
            sut.TotalJoltage().Should().Be(357);
        }
    }
    
    public class Day03Part1
    {
        [Fact]
        public void it_can_find_the_total_joltage()
        {
            var sut = new JoltageMaximiser("day3.txt");
            sut.TotalJoltage().Should().Be(17155);
        }
    }

    public class Day03Part2
    {
        [Theory]
        [InlineData("987654321111111", 987654321111)]
        [InlineData("811111111111119", 811111111119)] 
        [InlineData("234234234234278", 434234234278)]
        [InlineData("818181911112111", 888911112111)] 
        public void it_can_find_the_joltages(string batteries, long totalJoltage)
        {
            var range = new BatteryBank(batteries, 12);
            range.TotalJoltage().Should().Be(totalJoltage);
        }
        
        [Fact]
        public void it_can_find_the_total_test_joltage()
        {
            var sut = new JoltageMaximiser("day3_test.txt", 12);
            sut.TotalJoltage().Should().Be(3121910778619);
        }
 
        [Fact]
        public void it_can_find_the_total_joltage()
        {
            var sut = new JoltageMaximiser("day3.txt", 12);
            sut.TotalJoltage().Should().Be(169685670469164L);
        }
    }
}

public class JoltageMaximiser(string fileName, int numberOfBatteries = 2)
{
    public long TotalJoltage()
    {
        return FileReader.FromInput(fileName).Lines()
            .Select(line => new BatteryBank(line, numberOfBatteries).TotalJoltage()).Sum();
    }
}

public class BatteryBank
{
    private readonly StringBuilder result = new();
    
    public BatteryBank(string batteries, int numberOfBatteries = 2)
    {
        var nextHighestIndex = 0;
        var remaining = numberOfBatteries - 1;

        while (remaining >= 0)
        {
            var nextHighest = batteries[nextHighestIndex];
            
            for (int i = nextHighestIndex + 1; i < batteries.Length - remaining; i++)
            {
                if (batteries[i] > nextHighest)
                {
                    nextHighest = batteries[i];
                    nextHighestIndex = i;
                }
            }

            nextHighestIndex++;
            remaining--;
            result.Append(nextHighest);
        }
    }

    public long TotalJoltage()
    {
        return long.Parse(result.ToString());
    }
}

public class TwoBatteryBank
{
    public TwoBatteryBank(string batteries)
    {
        var firstHighestIndex = 0;
        var firstHighest = batteries[firstHighestIndex];
        
        for(int i = 1; i < batteries.Length - 1; i++)
        {
            if (batteries[i] > firstHighest)
            {
                firstHighest = batteries[i];
                firstHighestIndex = i;
            }
        }
        
        FirstJoltage = int.Parse(firstHighest.ToString());
        
        var secondHighest = batteries[firstHighestIndex + 1];
        
        for(int i = firstHighestIndex + 1; i < batteries.Length; i++)
        {
            if (batteries[i] > secondHighest)
            {
                secondHighest = batteries[i];
            }
        }
        
        SecondJoltage = int.Parse(secondHighest.ToString());
    }

    public int FirstJoltage { get; } = 0;

    public int SecondJoltage { get; } = 0;

    public int TotalJoltage()
    {
        return FirstJoltage * 10 + SecondJoltage;
    }
}