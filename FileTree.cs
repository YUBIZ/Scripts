public readonly record struct FileTree(string Name, FileTree[] SubTrees, string[] Files)
{
    public List<string> ToList()
    {
        List<string> fileList = [];
        ToList(ref fileList);
        return fileList;
    }

    private void ToList(ref List<string> fileList, string basePath = "")
    {
        foreach (var item in SubTrees)
        {
            item.ToList(ref fileList, Path.Combine(basePath, item.Name));
        }
        foreach (var item in Files)
        {
            fileList.Add(Path.Combine(basePath, item));
        }
    }
}
