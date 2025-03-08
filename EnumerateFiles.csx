#nullable enable

#load "Common.csx"

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

static void EnumerateFiles(string sourceDir, string outputFile, string[] searchPatterns, string[] excludePatterns, string? stripPath)
{
    stripPath ??= sourceDir;
    
    IEnumerable<string> files = GetFiles(sourceDir, searchPatterns, excludePatterns, SearchOption.AllDirectories)
                               .Select(f => Path.GetRelativePath(stripPath, f));

    JsonSerializerOptions jsonSerializerOptions = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    if (Path.GetDirectoryName(outputFile) is string outputDir && !string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    File.WriteAllText(outputFile, JsonSerializer.Serialize(files, jsonSerializerOptions));
}
