#load "Common.csx"
#load "Models/FilePathAndCommitHistory.csx"
#load "Models/SiteMap.csx"

using LibGit2Sharp;

public static void GenerateSiteMap(string sourceDirPath, string outputFilePath, string baseUrl, string[] searchPatterns, string[] excludePatterns)
{
    var repoPath = Repository.Discover(sourceDirPath);
    if (repoPath == null) return;

    using var repo = new Repository(repoPath);
    var repoRootDir = repo.Info.WorkingDirectory;

    var filePathInfos = EnumerateFilePathInfos(sourceDirPath, searchPatterns, excludePatterns);

    var filePathAndCommitHistoryCollection = filePathInfos.Select(v => new FilePathAndCommitHistory(v.RelativePath, EnumerateCommitHistory(repo, Path.GetRelativePath(repoRootDir, v.AbsolutePath).Replace("\\", "/")).ToArray()));

    var urlSet = filePathAndCommitHistoryCollection.Select(v => new Url(baseUrl + v.FilePath, v.CommitHistory.Max(v1 => v1.Date).ToString("yyyy-MM-dd"), null, null)).ToArray();

    GenerateXml(outputFilePath, urlSet, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
}
