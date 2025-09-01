#load "Common.csx"
#load "Models/FilePathAndCommitHistory.csx"
#load "Models/PathInfo.csx"

#nullable enable

using LibGit2Sharp;

static void GenerateFilePathAndCommitHistoryCollection(string sourceDirPath, string outputFilePath, string[] searchPatterns, string[] excludePatterns)
{
    var repoPath = Repository.Discover(sourceDirPath);
    if (repoPath == null) return;

    using var repo = new Repository(repoPath);
    var repoRootDir = repo.Info.WorkingDirectory;

    var filePathInfos = EnumerateFilePathInfos(sourceDirPath, searchPatterns, excludePatterns);

    var filePathAndCommitHistoryCollection = filePathInfos.Select(v => new FilePathAndCommitHistory(v.RelativePath, EnumerateCommitHistory(repo, Path.GetRelativePath(repoRootDir, v.AbsolutePath).Replace("\\", "/")).ToArray()));

    GenerateJson(outputFilePath, filePathAndCommitHistoryCollection);
}