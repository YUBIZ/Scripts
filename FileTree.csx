#load "Common.csx"

using System.Text.RegularExpressions;

public readonly record struct FileTree(string Dir, FileTree[] SubDirs, string[] Files)
{
    public Dictionary<string, FileTree> ToDictionary()
    {
        Dictionary<string, FileTree> fileTreeDict = [];
        ToDictionary(ref fileTreeDict);
        return fileTreeDict;
    }

    private void ToDictionary(ref Dictionary<string, FileTree> fileTreeDict, string basePath = "")
    {
        foreach (var item in SubDirs)
        {
            item.ToDictionary(ref fileTreeDict, Path.Combine(basePath, item.Dir));
        }
        foreach (var item in Files)
        {
            fileTreeDict.Add(Path.Combine(basePath, item), this);
        }
    }

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

static FileTree? GetFileTree(string dir)
{
    var subDirs = Directory.EnumerateDirectories(dir)
                           .Select(GetFileTree)
                           .Where(tree => tree.HasValue)
                           .Select(tree => tree!.Value)
                           .ToArray();

    var files = Directory.EnumerateFiles(dir)
                         .Select(Path.GetFileName)
                         .ToArray();

    return files.Length == 0 && subDirs.All(subDir => subDir.Files.Length == 0) ? null : new FileTree(Path.GetFileName(dir), subDirs, files);
}

static FileTree? GetFileTree(string dir, string[] searchPatterns, string[] excludePatterns)
{
    if (GetFileTree(dir) is not FileTree fileTree) return null;

    Dictionary<string, FileTree> fileTreeDict = fileTree.ToDictionary();

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        // @TODO: 탐색 필터링 구현
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        // @TODO: 제외 필터링 구현
    }

    return fileTree;
}
