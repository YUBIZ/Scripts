#r "nuget:System.CommandLine, 2.0.0-beta4.22272.1"

#load "GenerateFilePathWithCommitHistoryCollection.csx"
#load "GenerateFileTree.csx"

using System.CommandLine;

RootCommand rootCommand = new("스크립트 명령어를 실행합니다.");

// GenerateFilePathWithCommitHistoryCollection Command
{
    Argument<string> argument = new("source-dir-path", "탐색할 디렉터리 경로입니다.");
    Argument<string> argument1 = new("output-file-path", "결과를 저장할 JSON 파일 경로입니다.");

    Option<string[]> option = new("--search-patterns", "일치해야하는 파일 패턴 목록입니다.");
    Option<string[]> option1 = new("--exclude-patterns", "제외해야하는 파일 패턴 목록입니다.");

    var GenerateFilePathWithCommitHistoryCollectionCommand = new Command("generate-file-path-with-commit-history-collection", "파일 경로와 커밋 기록 컬렉션을 생성합니다.")
    {
        argument,
        argument1,
        option,
        option1
    };

    GenerateFilePathWithCommitHistoryCollectionCommand.SetHandler(GenerateFilePathWithCommitHistoryCollection, argument, argument1, option, option1);

    rootCommand.Add(GenerateFilePathWithCommitHistoryCollectionCommand);
}

// GenerateFileTree Command
{
    Argument<string> argument = new("source-dir-path", "탐색할 디렉터리 경로입니다.");
    Argument<string> argument1 = new("output-file-path", "결과를 저장할 JSON 파일 경로입니다.");

    Option<string[]> option = new("--search-patterns", "일치해야하는 파일 패턴 목록입니다.");
    Option<string[]> option1 = new("--exclude-patterns", "제외해야하는 파일 패턴 목록입니다.");

    var GenerateFileTreeCommand = new Command("generate-file-tree", "파일 트리를 생성합니다.")
    {
        argument,
        argument1,
        option,
        option1
    };

    GenerateFileTreeCommand.SetHandler(GenerateFileTree, argument, argument1, option, option1);

    rootCommand.Add(GenerateFileTreeCommand);
}

return await rootCommand.InvokeAsync([.. Args]);
