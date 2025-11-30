namespace MarkGravestock.AdventOfCode2025.Common;

public class FileReader(string fileName)
{
    public static FileReader FromInput(string fileName) => new FileReader($@"./Input/{fileName}");
    
    public IEnumerable<string> Lines()
    {
        using var sr = new StreamReader(fileName);
        var text = sr.ReadToEnd();
        var lines = text.Split("\n");
        return lines.Select(x => x.Trim()).Where(x => x != String.Empty);
    }
}