#load "Common.csx"

static void GenerateFileList(string sourceDir, string outputFile, string[] searchPatterns, string[] excludePatterns)
{
    IEnumerable<string> files = GetFileList(sourceDir, searchPatterns, excludePatterns, SearchOption.AllDirectories);

    GenerateJson(outputFile, files);
}
