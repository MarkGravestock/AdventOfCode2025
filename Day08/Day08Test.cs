using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day08;

public class Day08Test
{
    public class Day08Example
    {
        private readonly ITestOutputHelper output;

        public Day08Example(ITestOutputHelper output)
        {
            this.output = output;
            sut = new JunctionBoxConnector("day8_test.txt", output);
        }

        private readonly JunctionBoxConnector sut;

        [Fact]
        public void it_can_read_the_junction_box_coordinates()
        {
            sut.Coordinates().Count().Should().Be(20);
        }

        [Fact]
        public void it_can_find_the_nearest_junction_box()
        {
            sut.FindNearest(new Coordinate(162,817,812)).Should().Be(new Coordinate(425,690,689));
        }
        
        [Fact]
        public void it_can_find_the_nearest_candidate()
        {
            sut.FindNearest().First().Same(new Candidate(new Coordinate(425,690,689), new Coordinate(162,817,812))).Should().BeTrue();
        }

        [Fact]
        public void it_can_find_the_nearest_junction_boxes()
        {
            var nearest = sut.FindNearest().Select(x => (x, x.Distance));
            nearest.ForEach(x => output.WriteLine(x.ToString()));
        }
        
        [Fact]
        public void it_can_form_a_circuit()
        {
            var circuits = sut.AddToCircuits(new Candidate(new Coordinate(425, 690, 689), new Coordinate(162, 817, 812)));
            
            sut.AddToCircuits(new Candidate(new Coordinate(431, 825, 988), new Coordinate(162, 817, 812)));
            sut.AddToCircuits(new Candidate(new Coordinate(162, 817, 812), new Coordinate(431, 825, 988)));
            sut.AddToCircuits(new Candidate(new Coordinate(425, 690, 689), new Coordinate(431, 825, 988)));
            
            circuits.Count().Should().Be(1);
            circuits.First().Count.Should().Be(3);
        }
        
        [Fact]
        public void it_can_form_another_circuit()
        {
            var circuits = sut.AddToCircuits(new Candidate(new Coordinate(862, 61, 35), new Coordinate(984, 92, 344)));
            sut.AddToCircuits(new Candidate(new Coordinate(908, 360, 560), new Coordinate(984, 92, 344)));
            
            circuits.Count().Should().Be(1);
            circuits.First().Count.Should().Be(3);
        }
        
        [Fact]
        public void it_can_form_all_circuits()
        {
            var result = sut.AddNearestToCircuits();
            result.Count.Should().Be(4);
            var ordered = result.OrderByDescending(x => x.Count);
            ordered.First().Count.Should().Be(5);
        }

        [Fact] 
        public void it_can_calculate_the_total_of_the_largest_circuits()
        {
            var result = sut.CalculateTotalOfLargest();
            result.Should().Be(40);
        }

    }

    public class Day08Part1(ITestOutputHelper output)
    {
        private readonly JunctionBoxConnector sut = new("day8.txt", output);

        [Fact]
        public void it_can_calculate_the_total_of_the_largest_circuits()
        {
            var result = sut.CalculateTotalOfLargest(1000);
            result.Should().Be(105952L);
        }
    }

    public class Day08Part2(ITestOutputHelper output)
    {
        
        [Fact]
        public void it_can_form_all_circuits_for_test()
        {
            var sut = new JunctionBoxConnector("day8_test.txt", output);
            var last = sut.FindFirstCandidateThatFormsASingleCircuitOfSize(20);
            (last.Coordinate1.X * last.Coordinate2.X).Should().Be(25272);
        }
        

        [Fact]
        public void it_can_form_all_circuits()
        {
            var sut = new JunctionBoxConnector("day8.txt", output);
            var last = sut.FindFirstCandidateThatFormsASingleCircuitOfSize(1000);
            (last.Coordinate1.X * last.Coordinate2.X).Should().Be(975931446);
        }
    }

}

public class JunctionBoxConnector
{
    private readonly ITestOutputHelper output;
    private readonly List<Coordinate> coordinates;
    private readonly List<Circuit> circuits = new();

    private JunctionBoxConnector(List<string> lines, ITestOutputHelper output)
    {
        this.output = output;
        coordinates = lines.Select(x => x.Split(",")).Select(y => new Coordinate(int.Parse(y[0]), int.Parse(y[1]), int.Parse(y[2]))).ToList();
    }
    
    public JunctionBoxConnector(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().ToList(), output)
    {
        this.output = output;
    }
    
    public List<Coordinate> Coordinates()
    {
        return coordinates;
    }

    public IEnumerable<Candidate> FindNearest(int count = 10)
    {
        return FindAllNearest().Take(count);
    }
    
    public IEnumerable<Candidate> FindAllNearest()
    {
        var allCombinations = coordinates.SelectMany((first, i) => 
                coordinates.Skip(i + 1).Select(second => new Candidate(first, second)))
            .ToList();
        
        return allCombinations.OrderBy(x => x.Distance);
    }
    
    public Coordinate FindNearest(Coordinate coordinate)
    {
        return coordinates.Filter(x => !x.Equals(coordinate)).OrderBy(x => x.DistanceTo(coordinate)).First();
    }

    public List<Circuit> AddToCircuits(Candidate candidate)
    {
        var coordinateInJunction = circuits.Filter(x => x.IsConnectedTo(candidate)).ToList();

        if (coordinateInJunction.Count > 2)
        {
            throw new Exception("Too many junctions");    
        }
        
        if (coordinateInJunction.Count == 2)
        {
            coordinateInJunction.First().Merge(coordinateInJunction.Last());
            circuits.Remove(coordinateInJunction.Last());
            return circuits;
        }
        
        if (coordinateInJunction.Count == 1)
        {
            coordinateInJunction.First().Add(candidate);
            return circuits;
        }
        
        var newCircuit = new Circuit();
        newCircuit.Add(candidate);
        circuits.Add(newCircuit);
        return circuits;
    }

    public Candidate FindFirstCandidateThatFormsASingleCircuitOfSize(int targetCount = 20)
    {
        var allNearest = FindAllNearest();
        
        Candidate lastCandidate = null;
        int count = 0;
        
        foreach (var candidate in allNearest)
        { 
            lastCandidate = candidate;
            AddToCircuits(candidate);
            count++;
            
            if (candidate.Same(new Candidate(new Coordinate(216,146,977), new Coordinate(117, 168, 530))))
            {
                output.WriteLine($"{count} {candidate}");
            }
            
            if (circuits.Count == 1 && circuits.First().Count == targetCount) break;
        }

        return lastCandidate;
    }
    
    public List<Circuit> AddNearestToCircuits(int count = 10)
    {
        var firstTen = FindNearest(count).Take(count);
        firstTen.ForEach(x => AddToCircuits(x));
        return circuits;
    }

    public long CalculateTotalOfLargest(int count = 10)
    {
        return AddNearestToCircuits(count).OrderByDescending(x => x.Count).Take(3).Aggregate(1, (acc, x) => acc * x.Count);;
    }
}

public record Candidate(Coordinate Coordinate1, Coordinate Coordinate2)
{
    public bool Same(Candidate other) => Coordinate1.Equals(other.Coordinate1) && Coordinate2.Equals(other.Coordinate2) || Coordinate1.Equals(other.Coordinate2) && Coordinate2.Equals(other.Coordinate1);
    
    public double Distance => Coordinate1.DistanceTo(Coordinate2);
}

public class Circuit
{
    private readonly HashSet<Coordinate> coordinates = new();

    public bool IsConnectedTo(Candidate candidate)
    {
        return coordinates.Contains(candidate.Coordinate1) || coordinates.Contains(candidate.Coordinate2);
    }
    
    public int Count => coordinates.Count;
    
    public void Add(Candidate candidate)
    {
        coordinates.Add(candidate.Coordinate2);
        coordinates.Add(candidate.Coordinate1);
    }

    public void Merge(Circuit other)
    {
        other.coordinates.ForEach(x => coordinates.Add(x));
    }
}

public record Coordinate(int X, int Y, int Z)
{
    public double DistanceTo(Coordinate other)
    {
        return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2) + Math.Pow(Z - other.Z, 2));
    }
}


