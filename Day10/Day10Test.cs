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
        public void it_can_solve_second_example()
        {
            var machine = new Machine(".....","[...#.]", ["(0,2,3,4)","(2,3)","(0,4)","(0,1,2)","(1,2,3,4)"]);
            machine.PressButton(2);
            machine.PressButton(3);
            machine.PressButton(4);
            machine.CurrentIndicatorLights().Should().Be("...#.");
            machine.IsAtDesiredState().Should().Be(true);
        }

        [Fact]
        public void it_can_solve_third_example()
        {
            var machine = new Machine("......","[.###.#]", ["(0,1,2,3,4)","(0,3,4)","(0,1,2,4,5)","(1,2)"]);
            machine.PressButton(1);
            machine.PressButton(2);
            machine.CurrentIndicatorLights().Should().Be(".###.#");
            machine.IsAtDesiredState().Should().Be(true);
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
        
        [Fact]
        public void it_can_find_the_minimum()
        {
            sut.FindMininum(0).Should().Be(2);
            sut.FindMininum(1).Should().Be(3);
            sut.FindMininum(2).Should().Be(2);
        }
        
        [Fact]
        public void it_can_find_the_total_minimum()
        {
            sut.FindTotalMininum().Should().Be(7);
        }

    }

    public class Day10Part1(ITestOutputHelper output)
    {
        private readonly IndicatorLightDiagram sut = new("day10.txt", output);

        [Fact]
        public void it_can_find_the_total_minimum()
        {
            sut.FindTotalMininum().Should().Be(399);
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

    public int FindMininum(int machineIndex)
    {
        var machine = machines[machineIndex];

        int minimum = -1;
        
        for (var i = 1; i <= machine.ButtonWiring.Length && minimum == -1; i++)
        {
            var combinations = GetCombinations(machine.ButtonWiring.Length - 1, i).ToList();
            
            for (var j = 0; j < combinations.Length() ; j++)
            {
                combinations[j].ForEach(buttonIndex => machine.PressButton(buttonIndex));
                if (machine.IsAtDesiredState())
                {
                    minimum = i;
                    break;
                }

                machine.Reset();
            }
        }

        return minimum;
    }

    public static IEnumerable<int[]> GetCombinations(int n, int k)
    {
        return GetCombinationsHelper(0, n, k);
    }

    private static IEnumerable<int[]> GetCombinationsHelper(int start, int n, int k)
    {
        if (k == 0)
        {
            yield return new int[0];
            yield break;
        }
    
        for (int i = start; i <= n - k + 1; i++)
        {
            foreach (var combo in GetCombinationsHelper(i + 1, n, k - 1))
            {
                yield return new[] { i }.Concat(combo).ToArray();
            }
        }
    }

    public int FindTotalMininum()
    {
        var total = 0;

        for (int i = 0; i < machines.Count; i++)
        {
            total += FindMininum(i);
        }
        
        return total;
    }
}

public class Machine 
{
    public Machine(string initialIndicatorLights, string desiredIndicatorLights, string[] buttonWiring)
    {
        InitialIndicatorLights = initialIndicatorLights;
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

    public void Reset()
    {
        currentIndicatorLights = Lights.From(InitialIndicatorLights);
    }
}



