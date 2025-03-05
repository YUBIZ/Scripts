using System.Text.RegularExpressions;

static string[] GetFiles(string sourceDir, string[] searchPatterns, string[] excludePatterns, SearchOption searchOption)
{
    string[] files = Directory.GetFiles(sourceDir, "", searchOption);

    if (searchPatterns.Length > 0)
    {
        IEnumerable<Regex> searchRegexes = searchPatterns.Select(pattern => new Regex("^" + Regex.Escape(pattern)
                                                                                                 .Replace(@"\*", ".*")
                                                                                                 .Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase));
        files = files.Where(f => searchRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f)))).ToArray();
    }
    if (excludePatterns.Length > 0)
    {
        IEnumerable<Regex> excludeRegexes = excludePatterns.Select(pattern => new Regex("^" + Regex.Escape(pattern)
                                                                                                   .Replace(@"\*", ".*")
                                                                                                   .Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase));
        files = files.Where(f => !excludeRegexes.Any(regex => regex.IsMatch(Path.GetRelativePath(sourceDir, f)))).ToArray();
    }

    return files;
}