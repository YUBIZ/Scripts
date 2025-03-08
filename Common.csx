using System.Text.RegularExpressions;

static Regex GetRegex(string pattern)
{
    return new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase);
}

static string[] GetFileList(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    IEnumerable<string> files = Directory.EnumerateFiles(sourceDir, "", searchOption).Select(v => Path.GetRelativePath(sourceDir, v));

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        files = files.Where(v => searchRegexes.Any(regex => regex.IsMatch(v)));
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        files = files.Where(v => !excludeRegexes.Any(regex => regex.IsMatch(v)));
    }

    return files.ToArray();
}
