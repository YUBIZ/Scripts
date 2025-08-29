#r "nuget: LibGit2Sharp, 0.31.0"

#load "Common.csx"
#load "Models/FilePathWithCommitHistory.csx"
#load "Models/PathInfo.csx"

#nullable enable

using LibGit2Sharp;
using System.Text.RegularExpressions;

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

static void GenerateFilePathWithCommitHistoryCollection(string sourceDirPath, string outputFilePath, string[] searchPatterns, string[] excludePatterns)
{
    var repoPath = Repository.Discover(sourceDirPath);
    if (repoPath == null) return;

    using var repo = new Repository(repoPath);
    var repoRootDir = repo.Info.WorkingDirectory;

    var filePathInfos = EnumerateFilePathInfos(sourceDirPath, searchPatterns, excludePatterns);

    var filePathWithCommitHistoryCollection = filePathInfos.Select(v => new FilePathWithCommitHistory(v.RelativePath, EnumerateCommitHistory(repo, Path.GetRelativePath(repoRootDir, v.AbsolutePath).Replace("\\", "/")).ToArray()));

    GenerateJson(outputFilePath, filePathWithCommitHistoryCollection);
}