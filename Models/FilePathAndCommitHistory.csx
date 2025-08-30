#load "CommitMetadata.csx"

public readonly record struct FilePathAndCommitHistory(string FilePath, CommitMetadata[] CommitHistory);
