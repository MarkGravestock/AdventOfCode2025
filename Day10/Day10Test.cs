using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day10;

public class Day10Test
{
    public class Day10Example
    {
        public Day10Example(ITestOutputHelper output)
        {
            sut = new IndicatorLightDiagram("day10_test.txt", output);
        }

        private readonly IndicatorLightDiagram sut;
        

        [Fact]
        public void it_can_read_the_manual()
        {
            sut.Machines().First().DesiredIndicatorLights.Should().Be(".##.");
            sut.Machines().First().ButtonWiring.Length.Should().Be(6);
        }

        [Fact]
        public void it_apply_a_button_press()
        {
            var machine = new Machine("#.....","...##.", ["(0,3,4)"]);
            machine.PressButton(0);
            machine.CurrentIndicatorLights().Should().Be("...##.");
        }
        
        [Fact]
        public void it_apply_a_button_press_to_machine()
        {
            var machine = sut.Machines().First();
            machine.PressButton(0);
            machine.PressButton(1);
            machine.PressButton(2);
            machine.IsAtDesiredState().Should().Be(true);
        }
    }

    public class Day10Part1(ITestOutputHelper output)
    {
        private readonly IndicatorLightDiagram sut = new("day10.txt", output);

        [Fact]
        public void it_can_count_the_machines()
        {
            sut.Machines().Count.Should().Be(157);
        }
    }

    public class Day10Part2(ITestOutputHelper output)
    {
        
        [Fact]
        public void it_can_count_the_machines_in_examples()
        {
            var sut = new IndicatorLightDiagram("day10_test.txt", output);
            sut.Machines().Count.Should().Be(3);
        }

        
        [Fact]
        public void it_can_count_the_machines()
        {
            var sut = new IndicatorLightDiagram("day10.txt", output);
            sut.Machines().Count.Should().Be(157);
        }
    }

}

public class Lights
{
    private readonly int value;
    private readonly int length;

    public Lights(int value, int length)
    {
        this.value = value;
        this.length = length;
    }

    public static Lights From(string indicators)
    {
        return new Lights(Convert.ToInt32(new string(indicators.Replace(".", "0").Replace("#", "1").Reverse().ToArray()), 2), indicators.Length);
    }

    public int Value => value;
    public int Length => length;
    
    public override string ToString()
    {
        return new string(Convert.ToString(value, 2).PadLeft(length, '0').Replace("0", ".").Replace("1", "#").Reverse().ToArray());
    }
}

public class IndicatorLightDiagram
{
    private readonly ITestOutputHelper output;
    private readonly List<Machine> machines;

    public IndicatorLightDiagram(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
    }

    private IndicatorLightDiagram(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;

        machines = lines.Select(x => x.Split(" "))
            .Select(y => new Machine(desiredIndicatorLights: y[0], buttonWiring: y[1..^1])).ToList();
    }

    public List<Machine> Machines()
    {
        return machines;
    }

}

public class Machine 
{
    public Machine(string initialIndicatorLights, string desiredIndicatorLights, string[] buttonWiring)
    {
        this.InitialIndicatorLights = initialIndicatorLights;
        this.desiredIndicatorLights = desiredIndicatorLights;
        var wiring = buttonWiring.Select(x => x[1..^1]);
        this.buttonWiring = wiring.Select(BinaryPositionsToDecimal).ToArray();
        currentIndicatorLights = Lights.From(initialIndicatorLights);
    }
    public Machine(string desiredIndicatorLights, string[] buttonWiring) : this(
        "".PadLeft(desiredIndicatorLights.Length - 2, '.'), desiredIndicatorLights, buttonWiring)
    {
    }

    public static int BinaryPositionsToDecimal(string positions)
    {
        return positions.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .Sum(pos => 1 << pos); 
    }
    
    private Lights currentIndicatorLights;
    private readonly string desiredIndicatorLights;
    private readonly int[] buttonWiring;
    private readonly string joltage;

    public string InitialIndicatorLights { get; }

    public int[] ButtonWiring => buttonWiring;

    public string DesiredIndicatorLights => desiredIndicatorLights[1..^1];

    public void PressButton(int buttonIndex)
    {
        currentIndicatorLights = new Lights(buttonWiring[buttonIndex] ^ currentIndicatorLights.Value, currentIndicatorLights.Length);
    }

    public string CurrentIndicatorLights()
    {
        return currentIndicatorLights.ToString();
    }

    public bool IsAtDesiredState()
    {
        return CurrentIndicatorLights() == DesiredIndicatorLights;
    }
}

