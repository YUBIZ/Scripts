#r "nuget: LibGit2Sharp, 0.31.0"

#load "Models/Aliases.csx"

#nullable enable

using LibGit2Sharp;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Linq;

static void GenerateJson<T>(string outputFilePath, T value)
{
    if (Path.GetDirectoryName(outputFilePath) is string outputDirPath && !string.IsNullOrEmpty(outputDirPath) && !Directory.Exists(outputDirPath))
    {
        Directory.CreateDirectory(outputDirPath);
    }

    var jsonSerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    File.WriteAllText(outputFilePath, System.Text.Json.JsonSerializer.Serialize(value, jsonSerializerOptions));
}

static void GenerateXml<T>(string outputFilePath, T value, string rootName, string rootNamespace)
{
    if (Path.GetDirectoryName(outputFilePath) is string outputDirPath && !string.IsNullOrEmpty(outputDirPath) && !Directory.Exists(outputDirPath))
    {
        Directory.CreateDirectory(outputDirPath);
    }

    var serializer = new DataContractSerializer(typeof(T), rootName, rootNamespace);
    using var memoryStream = new MemoryStream();
    using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
    {
        serializer.WriteObject(writer, value);
    }
    memoryStream.Position = 0;
    XDocument xDoc = XDocument.Load(memoryStream);

    if (xDoc.Root == null) return;

    xDoc.Save(outputFilePath);
}

static Regex? GetRegex(string[] patterns)
{
    if (patterns.Length == 0) return null;

    string combinedPattern = string.Join("|", patterns.Select(p => $"({Regex.Escape(p).Replace(@"\*", ".*").Replace(@"\?", ".")})"));

    return new Regex($"^({combinedPattern})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}

static IEnumerable<PathInfo> EnumerateFilePathInfos(string sourceDirPath, string[] searchPatterns, string[] excludePatterns)
{
    sourceDirPath = Path.TrimEndingDirectorySeparator(sourceDirPath);

    string sourceDirFullPath = Path.GetFullPath(sourceDirPath);
    string sourceDirName = Path.GetFileName(sourceDirPath);

    var pathInfos = Directory.EnumerateFiles(sourceDirFullPath, "*", SearchOption.AllDirectories).Select(v => new PathInfo(v, Path.Combine(sourceDirName, Path.GetRelativePath(sourceDirFullPath, v))));

    if (GetRegex(searchPatterns) is Regex searchRegex)
    {
        pathInfos = pathInfos.Where(v => searchRegex.IsMatch(v.RelativePath));
    }
    if (GetRegex(excludePatterns) is Regex excludeRegex)
    {
        pathInfos = pathInfos.Where(v => !excludeRegex.IsMatch(v.RelativePath));
    }

    return pathInfos;
}

static IEnumerable<CommitMetadata> EnumerateCommitHistory(Repository repo, string repoRelativePath)
{
    var logEntries = repo.Commits.QueryBy(repoRelativePath, new CommitFilter { SortBy = CommitSortStrategies.Topological });

    return logEntries.Select(entry =>
    {
        var commit = entry.Commit;
        return new CommitMetadata(
            commit.Author.Name,
            commit.Author.Email,
            commit.Committer.Name,
            commit.Committer.Email,
            commit.Message,
            commit.Committer.When.DateTime
        );
    });
}

static FileTree GetFileTree(string sourceDirPath, string[] searchPatterns, string[] excludePatterns)
{
    sourceDirPath = Path.TrimEndingDirectorySeparator(sourceDirPath);

    string sourceDirFullPath = Path.GetFullPath(sourceDirPath);
    string rootDirName = Path.GetFileName(sourceDirPath);

    return GetFileTreeInternal(sourceDirFullPath, sourceDirFullPath, rootDirName, GetRegex(searchPatterns), GetRegex(excludePatterns));
}

static FileTree GetFileTreeInternal(string rootDirPath, string dirPath, string rootDirName, Regex? searchRegex, Regex? excludeRegex)
{
    var subTrees = Directory.EnumerateDirectories(dirPath).
                             AsParallel().
                             Select(v => GetFileTreeInternal(rootDirPath, v, rootDirName, searchRegex, excludeRegex)).
                             Where(v => v != default).
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

    if (files.Length == 0 && subTrees.Length == 0) return default;

    return new FileTree(new(Path.GetFileName(dirPath), files), subTrees);
}
