using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;

namespace MarkGravestock.AdventOfCode2025.Day01;

public class Day01Test
{
    public class Day01Example
    {
        [Fact]
        public void when_the_it_gets_a_rotation_it_applies_it_to_the_dial()
        {
            var dial = new Dial(startingAt: 50);
            dial.Rotate("L68").CurrentPosition().Should().Be(82);
            dial.Rotate("L30").CurrentPosition().Should().Be(52);
            dial.Rotate("R48").CurrentPosition().Should().Be(0);
        }

        [Fact]
        public void it_can_calculate_the_password()
        {
            var dial = new Dial(startingAt: 50);
            dial.Rotate("L68");
            dial.Rotate("L30");
            dial.Rotate("R48");

            dial.Password().Should().Be(1);
        }

        [Fact]
        public void it_can_calculate_the_password_from_the_example()
        {
            var lines = FileReader.FromInput("day1_test.txt").Lines();

            var dial = new Dial(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(3);
        }
    }
    
    public class Day01Part1
    {
        [Fact]
        public void it_deal_with_large_right_rotations()
        {
            var dial = new Dial(startingAt: 0);
            dial.Rotate("R577").CurrentPosition().Should().Be(77);
        }
 
        [Fact]
        public void it_deal_with_large_left_rotations()
        {
            var dial = new Dial(startingAt: 25);
            dial.Rotate("L725").CurrentPosition().Should().Be(0);
        }

        [Fact]
        public void it_can_calculate_the_password_from_the_file()
        {
            var lines = FileReader.FromInput("day1.txt").Lines();

            var dial = new Dial(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(1021);
        }
    }
    
    public class Day01Part2
    {
        [Fact]
        public void when_the_it_gets_a_rotation_it_calculates_the_password()
        {
            var dial = new DialPart2(startingAt: 50);
            dial.Rotate("L68");
            dial.Password().Should().Be(1);
            dial.Rotate("L30");
            dial.Password().Should().Be(1);
            dial.Rotate("R48").CurrentPosition().Should().Be(0);
            dial.Password().Should().Be(2);
            dial.Rotate("L5");
            dial.Password().Should().Be(2);
            dial.Rotate("R60");
            dial.Password().Should().Be(3);
        }
        
        
        [Fact]
        public void it_can_calculate_the_password_from_the_example()
        {
            var lines = FileReader.FromInput("day1_test.txt").Lines();

            var dial = new DialPart2(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(6);
        }
        
        [Fact]
        public void it_deal_with_large_right_rotations()
        {
            var dial = new DialPart2(startingAt: 50);
            dial.Rotate("R1000").CurrentPosition().Should().Be(50);
            dial.Password().Should().Be(10);
        }
 
        [Fact]
        public void it_deal_with_large_left_rotations()
        {
            var dial = new DialPart2(startingAt: 25);
            dial.Rotate("L725").CurrentPosition().Should().Be(0);
            dial.Password().Should().Be(8);
        }

        [Fact]
        public void it_can_calculate_the_password_from_the_file()
        {
            var lines = FileReader.FromInput("day1.txt").Lines();

            var dial = new DialPart2(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(5933);
        }
    }

}

public abstract class DialBase(int startingAt)
{
    protected int currentPosition = startingAt;
    protected int password;

    public DialBase Rotate(string rotation)
    {
        var clicks = int.Parse(rotation.Substring(1));
        var direction = rotation[0];
        
        UpdateDialState(direction, clicks);

        return this;
    }

    protected abstract void UpdateDialState(char direction, int clicks);

    public int CurrentPosition()
    {
        return currentPosition;
    }

    public int Password()
    {
        return password;
    }
}

public class Dial(int startingAt) : DialBase(startingAt)
{
    protected override void UpdateDialState(char direction, int clicks)
    {
        if (direction == 'L')
        {
            currentPosition -= clicks % 100;
            if (currentPosition < 0) currentPosition += 100;
        }
        else
        {
            currentPosition += clicks % 100;
            if (currentPosition >= 100) currentPosition -= 100;
        }

        if (currentPosition == 0) password++;
    }
}

public class DialPart2(int startingAt) : DialBase(startingAt)
{
    protected override void UpdateDialState(char direction, int clicks)
    {
        int numberOfPasses = clicks / 100;
        
        if (direction == 'L')
        {
            var lastPosition = currentPosition;
            currentPosition -= clicks % 100;
            if (currentPosition < 0)
            {
                currentPosition += 100;
                if (lastPosition != 0) password++;
            }
        }
        else
        {
            currentPosition += clicks % 100;
            if (currentPosition >= 100)
            {
                currentPosition -= 100;
                if (currentPosition != 0) password++;
            }
        }
        
        password += numberOfPasses;
        
        if (currentPosition == 0) password++;
    }
}