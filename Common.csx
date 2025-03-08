using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

static void GenerateJson<T>(string outputFile, T value)
{
    JsonSerializerOptions jsonSerializerOptions = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    if (Path.GetDirectoryName(outputFile) is string outputDir && !string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    File.WriteAllText(outputFile, JsonSerializer.Serialize(value, jsonSerializerOptions));
}

static Regex GetRegex(string pattern)
{
    return new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase);
}

static string[] GetFileList(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    IEnumerable<string> files = Directory.EnumerateFiles(sourceDir, "", searchOption).Select(v => Path.GetRelativePath(sourceDir, v));

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        files = files.Where(v => searchRegexes.Any(regex => regex.IsMatch(v)));
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        files = files.Where(v => !excludeRegexes.Any(regex => regex.IsMatch(v)));
    }

    return files.ToArray();
}
