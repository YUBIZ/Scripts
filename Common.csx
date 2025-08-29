#nullable enable

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;

static void GenerateJson<T>(string outputFile, T value)
{
    JsonSerializerOptions jsonSerializerOptions = new() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

    if (Path.GetDirectoryName(outputFile) is string outputDir && !string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    File.WriteAllText(outputFile, JsonSerializer.Serialize(value, jsonSerializerOptions));
}

static Regex? GetRegex(string[] patterns)
{
    if (patterns.Length == 0) return null;

    string combinedPattern = string.Join("|", patterns.Select(p => $"({Regex.Escape(p).Replace(@"\*", ".*").Replace(@"\?", ".")})"));

    return new Regex($"^({combinedPattern})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
