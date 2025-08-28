public readonly record struct FileTree(string Name, FileTree[] SubTrees, string[] Files)
{
    public IEnumerable<string> EnumerateFiles()
    {
        return EnumerateFilesInternal("");
    }

    private IEnumerable<string> EnumerateFilesInternal(string basePath)
    {
        foreach (var subTree in SubTrees)
        {
            foreach (var file in subTree.EnumerateFilesInternal( Path.Combine(basePath, subTree.Name))) yield return file;
        }

        foreach (var file in Files) yield return Path.Combine(basePath, file);
    }
}
