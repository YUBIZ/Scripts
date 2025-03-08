#nullable enable

#load "Common.csx"

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

static void GenerateFileList(string sourceDir, string outputFile, string[] searchPatterns, string[] excludePatterns)
{
    IEnumerable<string> files = GetFiles(sourceDir, searchPatterns, excludePatterns, SearchOption.AllDirectories);

    JsonSerializerOptions jsonSerializerOptions = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    if (Path.GetDirectoryName(outputFile) is string outputDir && !string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    File.WriteAllText(outputFile, JsonSerializer.Serialize(files, jsonSerializerOptions));
}
