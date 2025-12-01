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
            var dial = new DialMethod434C49434B(startingAt: 50);
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

            var dial = new DialMethod434C49434B(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(6);
        }
        
        [Fact]
        public void it_deal_with_large_right_rotations()
        {
            var dial = new DialMethod434C49434B(startingAt: 50);
            dial.Rotate("R1000").CurrentPosition().Should().Be(50);
            dial.Password().Should().Be(10);
        }
 
        [Fact]
        public void it_deal_with_large_left_rotations()
        {
            var dial = new DialMethod434C49434B(startingAt: 25);
            dial.Rotate("L725").CurrentPosition().Should().Be(0);
            dial.Password().Should().Be(8);
        }

        [Fact]
        public void it_can_calculate_the_password_from_the_file()
        {
            var lines = FileReader.FromInput("day1.txt").Lines();

            var dial = new DialMethod434C49434B(startingAt: 50);

            foreach (var line in lines)
            {
                dial.Rotate(line);
            }

            dial.Password().Should().Be(5933);
        }
    }

}



public class Dial
{
    private int currentPosition;
    private int password;

    public Dial(int startingAt)
    {
        currentPosition = startingAt;
    }

    public Dial Rotate(string rotation)
    {
        var clicks = int.Parse(rotation.Substring(1));
        var direction = rotation[0];
        
        if (direction == 'L')
        {
            currentPosition -= clicks % 100;
            if (currentPosition < 0) currentPosition = 100 + currentPosition;
        }
        else
        {
            currentPosition += clicks % 100;
            if (currentPosition >= 100) currentPosition -= 100;
        }
        
        if (currentPosition == 0) password++;
        
        return this;
    }
    
    public int CurrentPosition()
    {
        return currentPosition;
    }

    public int Password()
    {
        return password;
    }
}

public class DialMethod434C49434B
{
    private int currentPosition;
    private int password;

    public DialMethod434C49434B(int startingAt)
    {
        currentPosition = startingAt;
    }

    public DialMethod434C49434B Rotate(string rotation)
    {
        var clicks = int.Parse(rotation.Substring(1));
        var direction = rotation[0];
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
        
        return this;
    }
    
    public int CurrentPosition()
    {
        return currentPosition;
    }

    public int Password()
    {
        return password;
    }
}