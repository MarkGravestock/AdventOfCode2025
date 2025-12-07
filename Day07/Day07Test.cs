using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day07;

public class Day067Test
{
    public class Day07Example
    {
        public Day07Example(ITestOutputHelper output)
        {
            sut = new("day7_test.txt", output);
        }

        private readonly TachyonManifold sut;

        [Fact]
        public void it_can_read_the_bounds_of_the_diagram()
        {
            sut.Bounds().Height.Start.Value.Should().Be(0);
            sut.Bounds().Height.End.Value.Should().Be(7);
            sut.Bounds().Width.Start.Value.Should().Be(0);
            sut.Bounds().Width.End.Value.Should().Be(14);
        }

        [Fact]
        public void it_can_read_a_character_from_the_diagram()
        {
            sut.ReadAt(0, 7).Should().Be("S");
        }

        [Fact]
        public void it_can_find_the_start()
        {
            sut.FindStart().Should().Be(new Location(0, 7));
        }

        [Fact]
        public void it_can_find_the_first_splitter()
        {
            sut.FindNextSplitterFrom(sut.FindStart()).Should().Be(new Location(1, 7));
        }

        [Fact]
        public void it_signals_if_beam_leaves_manifold()
        {
            sut.FindNextSplitterFrom(new Location(0, 0)).Should().Be(Location.ExitedManifold());
        }


        [Fact]
        public void it_can_find_the_first_split_the_diagram()
        {
            sut.Split(sut.FindNextSplitterFrom(sut.FindStart())).Should()
                .BeEquivalentTo(new List<Location> { new(1, 6), new(1, 8) });
        }

        [Fact]
        public void it_can_simulate_the_beams()
        {
            var numberOfSplits = sut.SimulateBeams();

            numberOfSplits.Should().Be(21);
        }

    }

    public class Day07Part1(ITestOutputHelper output)
    {
        private readonly TachyonManifold sut = new("day7.txt", output);

        [Fact]
        public void it_can_simulate_the_beams()
        {
            var numberOfSplits = sut.SimulateBeams();

            numberOfSplits.Should().Be(1600);
        }
    }

    public class Day07Part2(ITestOutputHelper output)
    {
        
        [Fact]
        public void it_can_simulate_the_test_beams()
        {
            var sut = new TachyonManifold("day7_test.txt", output);
            
            var numberOfSplits = sut.SimulateQuantumBeams();

            numberOfSplits.Should().Be(40);
        }
        
        [Fact]
        public void it_can_simulate_the_test_beams_using_optimisation()
        {
            var sut = new TachyonManifold("day7_test.txt", output);
            
            var numberOfSplits = sut.SimulateQuantumBeamsOptimised();

            numberOfSplits.Should().Be(40);
        }

        [Fact]
        public void it_can_simulate_the_beams()
        {
            var sut = new TachyonManifold("day7.txt", output);
            
            var numberOfSplits = sut.SimulateQuantumBeamsOptimised();

            numberOfSplits.Should().Be(8632253783011L);
        }
    }

}

internal class TachyonManifold(List<string> inputLines)
{
    private readonly ITestOutputHelper output;

    List<string> diagram =  inputLines.Select(x => x).ToList();
    
    public TachyonManifold(string fileName, ITestOutputHelper output) : this(FileReader.FromInput(fileName).Lines().Filter(x => !x.All(y => y == '.')).ToList())
    {
        this.output = output;
    }

    public Bounds Bounds()
    {
        return new Bounds(new Range(0, inputLines.Count - 1), new Range(0, inputLines.First().Length - 1));
    }

    private void WriteAt(int line, int column, string value)
    {
        var chars = diagram[line].ToCharArray();
        chars[column] = value[0];
        diagram[line] = new string(chars);
    }

    public string ReadAt(int line, int column)
    {
        if (Bounds().Height.Contains(line) && Bounds().Width.Contains(column))
        {
            return inputLines[line][column].ToString();
        }
        
        return "X";
    }

    public Location FindStart()
    {
        return new Location(0, inputLines[0].IndexOf("S", StringComparison.Ordinal));
    }

    private Splitter FindNextSplitter(Splitter lastSplitter)
    {
        return new Splitter(FindNextSplitterFrom(lastSplitter.Location));
    }
    
    public Location FindNextSplitterFrom(Location findStart)
    {
        var currentLocation = findStart;

        while (ReadCurrent(currentLocation) != "^" && ReadCurrent(currentLocation) != "X")
        {
            WriteAt(currentLocation.Line.Value, currentLocation.Column.Value, "|");
            currentLocation = currentLocation.MoveDown();
        }
        
        return ReadCurrent(currentLocation) == "^" ? currentLocation : Location.ExitedManifold();
    }

    private string ReadCurrent(Location currentLocation)
    {
        return ReadAt(currentLocation.Line.Value, currentLocation.Column.Value);
    }

    public List<Location> Split(Location splitter)
    {
        List<Location> location = new();
        
        location.Add(new Location(splitter.Line.Value, splitter.Column.Value - 1));
        location.Add(new Location(splitter.Line.Value, splitter.Column.Value + 1));
        
        return location;
    }

    public long SimulateQuantumBeamsOptimised()
    {
        var startTime = DateTime.Now;
        
        var visitedSplitterCounts = new Dictionary<Splitter, long>();
        var startSplitter = new Splitter(FindNextSplitterFrom(FindStart()));

        var timelines =  CountTimelines(visitedSplitterCounts, startSplitter);
        OutputDiagram(0, timelines, startTime);
        return timelines;
    }

    private long CountTimelines(Dictionary<Splitter, long> splitterCounts,  Splitter currentSplitter)
    {
        if (splitterCounts.TryGetValue(currentSplitter, out var timelines)) return timelines;
        
        var nextSplitter = FindNextSplitter(currentSplitter);
        
        if (nextSplitter.Location == Location.ExitedManifold()) return splitterCounts[currentSplitter] = 1;
        
        var childCount = nextSplitter.Splits().Sum(split => CountTimelines(splitterCounts, new Splitter(FindNextSplitterFrom(split))));
        
        splitterCounts[nextSplitter] = childCount;

        return childCount;
    }

    public int SimulateQuantumBeams()
    {
        Location currentBeam = FindStart();
        
        HashSet<Location> splitsStillToTrace = new();
        splitsStillToTrace.Add(Location.ExitedManifold());
        var startTime = DateTime.Now;
        var timelines = 0;
        
        var iteration = 0;
        
        while (true)
        {
            iteration++;
            
            var beamToSplit = FindNextSplitterFrom(currentBeam);

            if (beamToSplit == Location.ExitedManifold())
            {
                
                if (splitsStillToTrace.Count == 0)
                {
                    break;
                }
                
                timelines++;
                currentBeam = splitsStillToTrace.Last();
                splitsStillToTrace.Remove(currentBeam);
                
                continue;
            };
            
            var splitBeams = Split(beamToSplit);
            currentBeam = splitBeams.First();

            splitsStillToTrace.Add(splitBeams.Last());
        }
        
        OutputDiagram(iteration, timelines, startTime);
        
        return timelines;
    }

    private void OutputDiagram(int iteration, long timelines, DateTime startTime)
    {
        OutputDiagram(iteration, 0, timelines, startTime);
    }

    public int SimulateBeams()
    {
        HashSet<Location> beams = new ();
        HashSet<Location> splitLocations = new();
        HashSet<Location> splitsAlreadyTraced = new();
        beams.Add(FindStart());
        var startTime = DateTime.Now;
        
        var iteration = 0;
        
        while (beams.Any())
        {
            iteration++;
            
            if (iteration % 100000 == 0) OutputDiagram(iteration, splitLocations.Count, beams.Count, startTime);
            
            var currentBeam = beams.Last();
            var beamToSplit = FindNextSplitterFrom(currentBeam);
            beams.Remove(currentBeam);
            
            if (beamToSplit == Location.ExitedManifold()) continue;
            
            splitLocations.Add(beamToSplit);
            
            var splitBeams = Split(beamToSplit);
            splitBeams.ForEach(x => {
                if (!splitsAlreadyTraced.Contains(x))
                {
                    beams.Add(x);
                    splitsAlreadyTraced.Add(x);
                }
            });
        }
        
        OutputDiagram(iteration, splitLocations.Count, beams.Count, startTime);
        
        return splitLocations.Count;
    }

    private void OutputDiagram(int iteration, int splitLocationsCount, long beamsCount, DateTime startTime)
    {
        var elapsedTime = DateTime.Now - startTime;
        var message =
            $"Output {iteration} Split locations: {splitLocationsCount} Beams: {beamsCount} Elapsed: {elapsedTime}";
        
        Console.WriteLine(message);
        diagram.ForEach(Console.WriteLine);
        
        output.WriteLine(message);
        diagram.ForEach(output.WriteLine);
    }
}

internal record Location(Index Line, Index Column)
{
    internal Location MoveDown()
    {
        return this with { Line = Line.Value + 1 };
    }

    public static Location ExitedManifold()
    {
        return new Location(int.MaxValue, int.MaxValue);
    }
}

internal record Splitter(Location Location)
{
    public List<Location> Splits()
    {
        List<Location> location = new();
        
        location.Add(new Location(Location.Line.Value, Location.Column.Value - 1));
        location.Add(new Location(Location.Line.Value, Location.Column.Value + 1));
        
        return location;
    }
}

internal record Bounds(Range Height, Range Width);

