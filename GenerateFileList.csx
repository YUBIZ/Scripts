#load "Common.csx"

static void GenerateFileList(string sourceDir, string outputFile, string[] searchPatterns, string[] excludePatterns)
{
    IEnumerable<string> files = EnumerateFiles(sourceDir, searchPatterns, excludePatterns, SearchOption.AllDirectories);

    GenerateJson(outputFile, files);
}
