using System.Collections.Generic;
using System.IO;

public static class Utils
{
    public static IEnumerable<string> ReadFromFile(string fileName) => File.ReadAllLines($"Resources/{fileName}");
}