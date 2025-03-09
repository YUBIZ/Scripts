#load "Common.csx"

using System.Text.RegularExpressions;

public readonly record struct FileTree(string Dir, FileTree[] SubDirs, string[] Files)
{
    public List<string> ToList()
    {
        List<string> fileList = [];
        ToList(ref fileList);
        return fileList;
    }

    private void ToList(ref List<string> fileList, string basePath = "")
    {
        foreach (var item in SubDirs)
        {
            item.ToList(ref fileList, Path.Combine(basePath, item.Dir));
        }
        foreach (var item in Files)
        {
            fileList.Add(Path.Combine(basePath, item));
        }
    }
}

static FileTree? GetFileTree(string dir, string[] searchPatterns, string[] excludePatterns)
{
    return GetFileTreeInternal(Path.GetFullPath(dir), Path.GetFullPath(dir), searchPatterns, excludePatterns);
}

static FileTree? GetFileTreeInternal(string rootDir, string dir, string[] searchPatterns, string[] excludePatterns)
{
    var subDirs = Directory.EnumerateDirectories(dir)
                           .Select(v => GetFileTreeInternal(rootDir, v, searchPatterns, excludePatterns))
                           .Where(v => v.HasValue)
                           .Select(v => v!.Value);

    var files = Directory.EnumerateFiles(dir).Select(v => Path.GetRelativePath(rootDir, v));

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        files = files.Where(file => searchRegexes.Any(regex => regex.IsMatch(file)));
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        files = files.Where(file => !excludeRegexes.Any(regex => regex.IsMatch(file)));
    }

    return !files.Any() && !subDirs.Any() ? null : new FileTree(Path.GetFileName(dir), subDirs.ToArray(), files.Select(Path.GetFileName).ToArray());
}
