#load "FileTree.cs"

#nullable enable

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

static Regex? GetRegex(string[] patterns)
{
    if (patterns.Length == 0) return null;

    string combinedPattern = string.Join("|", patterns.Select(p => $"({Regex.Escape(p).Replace(@"\*", ".*").Replace(@"\?", ".")})"));

    return new Regex($"^({combinedPattern})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}

static IEnumerable<string> EnumerateFiles(string sourceDirPath, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    sourceDirPath = Path.TrimEndingDirectorySeparator(sourceDirPath);

    string sourceDirName = Path.GetFileName(sourceDirPath);
    var files = Directory.EnumerateFiles(sourceDirPath, "", searchOption).Select(v => Path.Combine(sourceDirName, Path.GetRelativePath(sourceDirPath, v)));

    if (GetRegex(searchPatterns) is Regex searchRegex)
    {
        files = files.Where(v => searchRegex.IsMatch(v));
    }
    if (GetRegex(excludePatterns) is Regex excludeRegex)
    {
        files = files.Where(v => !excludeRegex.IsMatch(v));
    }

    return files;
}

static FileTree? GetFileTree(string sourceDirPath, string[] searchPatterns, string[] excludePatterns)
{
    sourceDirPath = Path.TrimEndingDirectorySeparator(sourceDirPath);
    string sourceDirFullPath = Path.GetFullPath(sourceDirPath);
    string rootDirName = Path.GetFileName(sourceDirPath);

    return GetFileTreeInternal(sourceDirFullPath, sourceDirFullPath, rootDirName, GetRegex(searchPatterns), GetRegex(excludePatterns));
}

static FileTree? GetFileTreeInternal(string rootDirPath, string dirPath, string rootDirName, Regex? searchRegex, Regex? excludeRegex)
{
    var subTrees = Directory.EnumerateDirectories(dirPath).
                             AsParallel().
                             Select(v => GetFileTreeInternal(rootDirPath, v, rootDirName, searchRegex, excludeRegex)).
                             Where(v => v.HasValue).
                             Select(v => v!.Value).
                             ToArray();

    var EnumerableFiles = Directory.EnumerateFiles(dirPath).
                          Select(v => Path.Combine(rootDirName, Path.GetRelativePath(rootDirPath, v)));

    if (searchRegex != null)
    {
        EnumerableFiles = EnumerableFiles.Where(v => searchRegex.IsMatch(v));
    }

    if (excludeRegex != null)
    {
        EnumerableFiles = EnumerableFiles.Where(v => !excludeRegex.IsMatch(v));
    }

    var files = EnumerableFiles.Select(Path.GetFileName).ToArray();

    if (files.Length == 0 && subTrees.Length == 0) return null;

    return new FileTree(Path.GetFileName(dirPath), subTrees, files);
}
