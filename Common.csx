#load "FileTree.cs"

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

static IEnumerable<string> EnumerateFiles(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    IEnumerable<string> files = Directory.EnumerateFiles(sourceDir, "", searchOption).Select(v => Path.GetRelativePath(".", v));

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

    return files;
}

static FileTree? GetFileTree(string sourceDir, string[] searchPatterns, string[] excludePatterns)
{
    return GetFileTreeInternal(Path.GetFullPath(sourceDir), Path.GetFullPath(sourceDir), searchPatterns.Select(GetRegex), excludePatterns.Select(GetRegex));
}

static FileTree? GetFileTreeInternal(string rootDir, string dir, IEnumerable<Regex> searchRegexes, IEnumerable<Regex> excludeRegexes)
{
    var subTrees = Directory.EnumerateDirectories(dir)
                            .Select(v => GetFileTreeInternal(rootDir, v, searchRegexes, excludeRegexes))
                            .Where(v => v.HasValue)
                            .Select(v => v!.Value);

    var files = Directory.EnumerateFiles(dir).Select(v => Path.GetRelativePath(rootDir, v));

    if (searchRegexes.Any())
    {
        files = files.Where(file => searchRegexes.Any(regex => regex.IsMatch(file)));
    }

    if (excludeRegexes.Any())
    {
        files = files.Where(file => !excludeRegexes.Any(regex => regex.IsMatch(file)));
    }

    return !files.Any() && !subTrees.Any() ? null : new FileTree(Path.GetFileName(dir), subTrees.ToArray(), files.Select(Path.GetFileName).ToArray());
}
