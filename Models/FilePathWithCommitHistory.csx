#load "CommitMetadata.csx"

public readonly record struct FilePathWithCommitHistory(string FilePath, CommitMetadata[] CommitHistory);
