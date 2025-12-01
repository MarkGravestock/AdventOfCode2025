using FluentAssertions;

namespace MarkGravestock.AdventOfCode2025.Day01;

public class Day01Test
{
    [Fact]
    public void when_the_it_gets_a_rotation_it_applies_it_to_the_dial()
    {
        var dial = new Dial(startingAt: 50);
        dial.Rotate("L68").CurrentPosition().Should().Be(82);
    }
}

public class Dial
{
    private int currentPosition;

    public Dial(int startingAt)
    {
        currentPosition = startingAt;
    }

    public Dial Rotate(string rotation)
    {
        var clicks = int.Parse(rotation.Substring(1));
        
        currentPosition = currentPosition - clicks;
        
        if (currentPosition < 0) currentPosition = 100 + currentPosition;
        
        return this;
    }
    
    public int CurrentPosition()
    {
        return currentPosition;
    }
}