using FluentAssertions;
using MarkGravestock.AdventOfCode2025.Common;
using Xunit.Abstractions;

namespace MarkGravestock.AdventOfCode2025.Day11;

public class Day11Test
{
    public class Day11Example
    {
        public Day11Example(ITestOutputHelper output)
        {
            sut = new Devices("day11_test.txt", output);
        }

        private readonly Devices sut;
        

        [Fact]
        public void it_can_read_the_devices()
        {
            sut.Paths().Should().Be(5);
        }

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Paths().Should().Be(5);
        }
    }

    public class Day11Part1(ITestOutputHelper output)
    {
        private readonly Devices sut = new("day11.txt", output);

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Paths().Should().Be(746);
        }
    }

    public class Day11Part2(ITestOutputHelper output)
    {
        [Fact]
        public void it_can_count_the_machines_in_examples()
        {
            var sut = new DevicesPart2("day11_test2.txt", output);
            sut.PathsViaDacAndFft().Should().Be(2);
        }

        
        [Fact]
        public void it_can_count_the_machines()
        {
            var sut = new DevicesPart2("day11.txt", output);
            sut.PathsViaDacAndFft().Should().Be(370500293582760L);
        }
    }

}

public class Devices
{
    private readonly ITestOutputHelper output;
    private readonly Dictionary<string, Device> devices = new();

    private readonly HashSet<Device> pathSet = new();
    private readonly List<Device> currentPath = new();
    private readonly List<List<Device>> allPaths = new();
    
    private Dictionary<(Device device, bool dacVisited, bool fftVisited), Device> memo = new();
    
    public Devices(string fileName, ITestOutputHelper output)
    {
        this.output = output;

        var lines = FileReader.FromInput(fileName).Lines().ToList();
        lines.ForEach(x =>
        {
            var name = x[..x.IndexOf(":", StringComparison.Ordinal)];
            var outputs = x[(x.IndexOf(":", StringComparison.Ordinal) + 2)..].Split(" ").ToList();
            devices.Add(name, new Device(name, outputs));
        });
        
        devices.Add("out", new Device("out", new List<string>()));
    }
    
    public long PathsViaDacAndFft()
    {
        var target = devices["out"];
        var currentDevice = devices["svr"];
        
        FindAllPaths(currentDevice, target, anyRoute: false);
        
        return allPaths.Count;
    }

    public long Paths()
    {
        var target = devices["out"];
        var currentDevice = devices["you"];
        
        FindAllPaths(currentDevice, target);
        
        return allPaths.Count;
    }

    private void FindAllPaths(Device currentDevice, Device target, bool anyRoute = true)
    {
        if (!pathSet.Add(currentDevice)) return;

        currentPath.Add(currentDevice);

        if (currentDevice == target)
        {
            if (anyRoute || (currentPath.Any(x => x.Name == "fft") && currentPath.Any(x => x.Name == "dac")))  
            {
                output.WriteLine($"Found match: {string.Join(" -> ", currentPath.Select(x => x.Name))}");
                allPaths.Add(new List<Device>(currentPath));
            }
        }
        else
        {
            foreach (var neighbour in currentDevice.Outputs)
            {
                var neighbourDevice = devices[neighbour];

                if (neighbourDevice.Name == "svr")
                {
                    output.WriteLine($"Help");
                }

                FindAllPaths(neighbourDevice, target, anyRoute);
            }
        }
        
        currentPath.RemoveAt(currentPath.Count - 1);
        pathSet.Remove(currentDevice);
    }
}

public class DevicesPart2
{
    private readonly ITestOutputHelper output;
    private readonly Dictionary<string, Device> devices = new();
    
    private readonly HashSet<Device> mustVisit = new();
    private readonly Dictionary<string, long> memo = new();
    
    int counter = 0;
    
    public DevicesPart2(string fileName, ITestOutputHelper output)
    {
        this.output = output;

        var lines = FileReader.FromInput(fileName).Lines().ToList();
        lines.ForEach(x =>
        {
            var name = x[..x.IndexOf(":", StringComparison.Ordinal)];
            var outputs = x[(x.IndexOf(":", StringComparison.Ordinal) + 2)..].Split(" ").ToList();
            devices.Add(name, new Device(name, outputs));
        });
        
        devices.Add("out", new Device("out", new List<string>()));
    }
    
    public long PathsViaDacAndFft()
    {
        var target = devices["out"];
        var start = devices["svr"];

        mustVisit.Add(devices["fft"]);
        mustVisit.Add(devices["dac"]);

        HashSet<Device> visited = new();
        HashSet<Device> requiredVisited = new();

        
        return CountMatchingPaths(start, target, visited, requiredVisited);
    }

    private long CountMatchingPaths(Device current, Device target, HashSet<Device> visited, HashSet<Device> requiredVisited)
    {
        counter++;
        
        if (counter % 100000 == 0)
        {
            output.WriteLine($"Calls: {counter:N0}");
        }
        
        if (current == target)
        {
            return mustVisit.IsSubsetOf(requiredVisited) ? 1 : 0;
        }
        
        var requiredVisitedList = string.Join(",", requiredVisited.OrderBy(x => x.Name));
        var key = $"{current.Name}:{requiredVisitedList}";

        if (memo.TryGetValue(key, out var paths)) 
            return paths;
        
        if (!visited.Add(current)) return 0;

        if (mustVisit.Contains(current))
        {
            requiredVisited.Add(current);
        }

        long count = 0;
        foreach (var neighbour in current.Outputs)
        {
            var neighbourDevice = devices[neighbour];
            count += CountMatchingPaths(neighbourDevice, target, visited, requiredVisited);
        }
        
        visited.Remove(current);
        requiredVisited.Remove(current);

        memo[key] = count;
        return count;
    }
}

internal class Device(string name, List<string> outputs)
{
    public string Name { get; } = name;
    public List<string> Outputs { get; } = outputs;
}


