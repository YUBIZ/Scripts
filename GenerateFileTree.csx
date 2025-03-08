#load "Common.csx"
#load "FileTree.csx"

public static void GenerateFileTree(string sourceDir, string outputFile)
{
    var fileTree = GetFileTree(sourceDir);

    GenerateJson(outputFile, fileTree);
}