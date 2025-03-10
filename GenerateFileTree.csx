#load "Common.csx"

public static void GenerateFileTree(string sourceDir, string outputFile, string[] searchPatterns, string[] excludePatterns)
{
    var fileTree = GetFileTree(sourceDir, searchPatterns, excludePatterns);

    GenerateJson(outputFile, fileTree);
}
