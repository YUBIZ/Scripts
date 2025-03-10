#load "Common.csx"

using System.Text.RegularExpressions;

public readonly record struct FileTree(string Name, FileTree[] SubTrees, string[] Files)
{
    public List<string> ToList()
    {
        List<string> fileList = [];
        ToList(ref fileList);
        return fileList;
    }

    private void ToList(ref List<string> fileList, string basePath = "")
    {
        foreach (var item in SubTrees)
        {
            item.ToList(ref fileList, Path.Combine(basePath, item.Name));
        }
        foreach (var item in Files)
        {
            fileList.Add(Path.Combine(basePath, item));
        }
    }
}

static FileTree? GetFileTree(string sourceDir, string[] searchPatterns, string[] excludePatterns)
{
    return GetFileTreeInternal(Path.GetFullPath(sourceDir), Path.GetFullPath(sourceDir), searchPatterns.Select(GetRegex), excludePatterns.Select(GetRegex));
}

static FileTree? GetFileTreeInternal(string rootDir, string dir, IEnumerable<Regex> searchRegexes, IEnumerable<Regex> excludeRegexes)
{
    var subDirs = Directory.EnumerateDirectories(dir)
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

    return !files.Any() && !subDirs.Any() ? null : new FileTree(Path.GetFileName(dir), subDirs.ToArray(), files.Select(Path.GetFileName).ToArray());
}
