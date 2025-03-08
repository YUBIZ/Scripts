using System.Text.RegularExpressions;

static Regex GetRegex(string pattern)
{
    return new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase);
}

static string[] GetFileList(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    IEnumerable<string> files = Directory.EnumerateFiles(sourceDir, "", searchOption);

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        files = files.Where(f => searchRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f))));
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        files = files.Where(f => !excludeRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f))));
    }

    return files.ToArray();
}
