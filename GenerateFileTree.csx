#load "Common.csx"

#nullable enable

public static void GenerateFileTree(string sourceDirPath, string outputFilePath, string[] searchPatterns, string[] excludePatterns)
{
    var fileTree = GetFileTree(sourceDirPath, searchPatterns, excludePatterns);

    GenerateJson(outputFilePath, fileTree);
}
