namespace MarkGravestock.AdventOfCode2025.Common;

public class FileReader(string fileName)
{
    public static FileReader FromInput(string fileName) => new($@"./Input/{fileName}");
    
    public IEnumerable<string> Lines()
    {
        var lines = AllLines();
        return lines.Select(x => x.Trim()).Where(x => x != String.Empty);
    }
    
    public IEnumerable<string> AllLines()
    {
        using var sr = new StreamReader(fileName);
        var text = sr.ReadToEnd();
        return text.Split("\n").ToList();
    }

}