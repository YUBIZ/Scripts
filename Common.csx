using System.Text.RegularExpressions;

static Regex GetRegex(string pattern)
{
    return new Regex("^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase);
}

static string[] GetFileList(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    string[] files = Directory.GetFiles(sourceDir, "", searchOption);

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(GetRegex);
        files = files.Where(f => searchRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f)))).ToArray();
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(GetRegex);
        files = files.Where(f => !excludeRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f)))).ToArray();
    }

    return files;
}
