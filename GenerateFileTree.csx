#load "Common.csx"
#load "Models/Aliases.csx"

#nullable enable

using System.Text.RegularExpressions;

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

    return new FileTree(new(Path.GetFileName(dirPath), files), subTrees);
}

public static void GenerateFileTree(string sourceDirPath, string outputFilePath, string[] searchPatterns, string[] excludePatterns)
{
    var fileTree = GetFileTree(sourceDirPath, searchPatterns, excludePatterns);

    GenerateJson(outputFilePath, fileTree);
}
