using FluentAssertions;
using Google.OrTools.LinearSolver;
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
        public void it_apply_a_button_press()
        {
            var machine = new DecimalMachine("{3,5,4,7}", ["(3)","(1,3)","(2)","(2,3)","(0,2)","(0,1)"]);

            machine.PressButton(1);
            machine.CurrentJoltage().Should().Be("{0,1,0,1}");
            machine.IsAtDesiredState().Should().Be(false);
        }
        
        [Fact]
        public void it_can_get_to_the_desired_state()
        {
            var machine = new DecimalMachine("{3,5,4,7}", ["(3)","(1,3)","(2)","(2,3)","(0,2)","(0,1)"]);

            machine.PressButton(0);
            machine.PressButton(1);
            machine.PressButton(1);
            machine.PressButton(1);
            machine.PressButton(3);
            machine.PressButton(3);
            machine.PressButton(3);
            machine.PressButton(4);
            machine.PressButton(5);
            machine.PressButton(5);
            machine.IsAtDesiredState().Should().Be(true);
        }

        [Fact]
        public void it_find_the_minimum_presses()
        {
            var sut = new IndicatorLightDiagramBruteForce("day10_test.txt", output);
            sut.FindMininum(0).Should().Be(10);
        }

        [Fact]
        public void it_can_count_the_machines_in_examples()
        {
            var sut = new IndicatorLightDiagramBruteForce("day10_test.txt", output);
            sut.FindTotalMininum().Should().Be(33);
        }

        
        [Fact(Skip = "Takes too long. Computationally unfeasible.")]
        public void it_can_count_the_machines()
        {
            var sut = new IndicatorLightDiagramBruteForce("day10.txt", output);
            sut.FindTotalMininum().Should().Be(157);
        }
        
        [Fact]
        public void it_find_the_minimum_presses_using_integer_linear_programming()
        {
            var sut = new IndicatorLightDiagramIntegerLinearProgramming("day10_test.txt", output);
            sut.FindMininum(0).Should().Be(10);
        }
        
        [Fact]
        public void it_find_the_minimum_total_presses_using_integer_linear_programming()
        {
            var sut = new IndicatorLightDiagramIntegerLinearProgramming("day10_test.txt", output);
            sut.FindTotalMininum().Should().Be(33);
        }
        
        [Fact]
        public void it_can_count_the_machines_using_integer_linear_programming()
        {
            var sut = new IndicatorLightDiagramIntegerLinearProgramming("day10.txt", output);
            sut.FindTotalMininum().Should().Be(15631);
        }

    }

}

public record SimpleMachine(int[][] operations, int[] desiredJoltage);


public class IndicatorLightDiagramIntegerLinearProgramming
{
    private readonly ITestOutputHelper output;
    private readonly SimpleMachine[] machines;

    public IndicatorLightDiagramIntegerLinearProgramming(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
    }
    
    private IndicatorLightDiagramIntegerLinearProgramming(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;
        
        var operations = lines.Select(x => x.Split(" ")).ToList();

        machines = operations.Select(y =>
        {
            var desiredJoltageString = y[^1];
            
            var buttonWiringString = y[1..^1];

            var desiredJoltage = desiredJoltageString[1..^1].Split(",").Select(int.Parse).ToArray();
            var buttonWiring = buttonWiringString.Select(x => x[1..^1])
                .Select(x => x.Split(",").Select(int.Parse).ToArray());

            var operations = buttonWiring.Select(wiring => Enumerable.Range(0, desiredJoltage.Length)
                .Select(i => wiring.Contains(i) ? 1 : 0)
                .ToArray()).ToArray();
            
            return new SimpleMachine(operations, desiredJoltage);
        }).ToArray();
        
    }

    public int FindMininum(int machineIndex)
    {
        return OperationSolverOrTools.FindMinimum(machines[machineIndex].operations, machines[machineIndex].desiredJoltage).Sum();
    }


    public int FindTotalMininum()
    {
        var total = 0;

        for (int i = 0; i < machines.Length; i++)
        {
            total += FindMininum(i);
            output.WriteLine($"{i} {total}");
        }
        
        return total;        
    }
}

public class OperationSolverOrTools
{
    public static int[] FindMinimum(int[][] operations, int[] target)
    {
        var solver = Solver.CreateSolver("SCIP");
        
        int numOps = operations.Length;
        int arraySize = target.Length;
        
        Variable[] counts = new Variable[numOps];
        for (int i = 0; i < numOps; i++)
        {
            counts[i] = solver.MakeIntVar(0, double.PositiveInfinity, $"op{i}");
        }
        
        for (int pos = 0; pos < arraySize; pos++)
        {
            Constraint constraint = solver.MakeConstraint(target[pos], target[pos]);
            for (int op = 0; op < numOps; op++)
            {
                constraint.SetCoefficient(counts[op], operations[op][pos]);
            }
        }
        
        Objective objective = solver.Objective();
        for (int i = 0; i < numOps; i++)
        {
            objective.SetCoefficient(counts[i], 1);
        }
        
        objective.SetMinimization();
        
        if (solver.Solve() != Solver.ResultStatus.OPTIMAL)
        {
            throw new Exception("No solution found");
        }
        
        return counts.Select(c => (int)Math.Round(c.SolutionValue())).ToArray();
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

public class IndicatorLightDiagramBruteForce
{
    private readonly ITestOutputHelper output;
    private readonly List<DecimalMachine> machines;

    public IndicatorLightDiagramBruteForce(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
    }

    private IndicatorLightDiagramBruteForce(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;

        machines = lines.Select(x => x.Split(" "))
            .Select(y => new DecimalMachine(desiredJoltage: y[^1], buttonWiring: y[1..^1])).ToList();
    }

    public int FindMininum(int machineIndex)
    {
        var machine = machines[machineIndex];

        int minimum = -1;
        
        for (var i = 1; i <= 1000 && minimum == -1; i++)
        {
            var combinations = GetCombinationsWithRepetition(machine.ButtonWiring.Length - 1, i).ToList();
            
            if (i % 20 == 0) output.WriteLine($"{i} {machineIndex}");
                
            for (var j = 0; j < combinations.Length() ; j++)
            {
                combinations[j].ForEach(buttonIndex => machine.PressButton(buttonIndex));

                if (machine.IsAtDesiredState())
                {
                    minimum = i;
                    output.WriteLine($"Machine: {machineIndex}");
                    break;
                }

                machine.Reset();
            }
        }

        if (minimum == -1) throw new Exception("No solution found");
        
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
    
    public static IEnumerable<int[]> GetCombinationsWithRepetition(int maxButton, int k)
    {
        int[] combination = new int[k];
        return GenerateCombinations(combination, 0, 0, maxButton);
    }

    private static IEnumerable<int[]> GenerateCombinations(int[] combination, int position, int start, int maxButton)
    {
        if (position == combination.Length)
        {
            yield return (int[])combination.Clone();
            yield break;
        }
    
        for (int i = start; i <= maxButton; i++)
        {
            combination[position] = i;
            foreach (var result in GenerateCombinations(combination, position + 1, i, maxButton))
            {
                yield return result;
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

public class DecimalMachine 
{
    private readonly int[] desiredJoltage;
    private int[] currentJoltage;

    public DecimalMachine(string desiredJoltage, string[] buttonWiring)
    {
        var desiredJoltageValue = desiredJoltage[1..^1];
        this.desiredJoltage = desiredJoltageValue.Split(",").Select(int.Parse).ToArray();
        currentJoltage = new int[this.desiredJoltage.Length];
        var wiring = buttonWiring.Select(x => x[1..^1]);
        this.buttonWiring = wiring.Select(x => x.Split(",").Select(int.Parse).ToArray()).ToArray();
    }
    
    private readonly int[][] buttonWiring;

    public int[][] ButtonWiring => buttonWiring;

    public void PressButton(int buttonIndex)
    {
        ButtonWiring[buttonIndex].ForEach(joltage => currentJoltage[joltage]++);
    }


    public bool IsAtDesiredState()
    {
        return currentJoltage.SequenceEqual(desiredJoltage);
    }

    public void Reset()
    {
        currentJoltage = new int[desiredJoltage.Length];
    }

    public string CurrentJoltage()
    {
        return "{" + string.Join(",", currentJoltage) + "}";
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

