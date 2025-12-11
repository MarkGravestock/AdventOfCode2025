using System.Collections;
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

    public class Day10Part1(ITestOutputHelper output)
    {
        private readonly Devices sut = new("day11.txt", output);

        [Fact]
        public void it_can_find_the_paths()
        {
            sut.Paths().Should().Be(746);
        }
    }

    public class Day10Part2(ITestOutputHelper output)
    {
        [Fact]
        public void it_can_count_the_machines_in_examples()
        {
            var sut = new Devices("day11_test.txt", output);
            sut.Should().NotBe(null);
        }

        
        [Fact]
        public void it_can_count_the_machines()
        {
            var sut = new Devices("day11.txt", output);
            sut.Should().NotBe(null);
        }
    }

}

public class Devices
{
    private readonly ITestOutputHelper output;
    private readonly Dictionary<string, Device> devices = new();
    
    private HashSet<Device> visited = new();
    private List<Device> currentPath = new();
    private List<List<Device>> allPaths = new();
    
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

    public long Count()
    {
        return devices.Count();
    }

    public long Paths()
    {
        var target = devices["out"];
        var currentDevice = devices["you"];
        
        FindAllPaths(currentDevice, target);
        
        return allPaths.Count;
    }

    private void FindAllPaths(Device currentDevice, Device target)
    {
        visited.Add(currentDevice);
        currentPath.Add(currentDevice);

        if (currentDevice == target)
        {
            allPaths.Add(new List<Device>(currentPath));
        }
        else
        {
            foreach (var neighbour in currentDevice.Outputs)
            {
                var neighbourDevice = devices[neighbour];

                if (!visited.Contains(neighbourDevice))
                {
                    FindAllPaths(neighbourDevice, target);
                }
            }
        }
        
        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Remove(currentDevice);
    }
}

internal class Device(string name, List<string> outputs)
{
    public List<string> Outputs { get; } = outputs;
}


