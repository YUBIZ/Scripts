#nullable enable

#r "nuget:System.CommandLine, 2.0.0-beta4.22272.1"

#load "EnumerateFiles.csx"

using System.CommandLine;

RootCommand rootCommand = new("스크립트 명령어를 실행합니다.");

// EnumerateFiles Command
{
    Argument<string> argument = new("source-dir", "탐색할 디렉터리입니다.");
    Argument<string> argument1 = new("output-path", "결과를 저장할 JSON 파일입니다.");

    Option<string[]> option = new("--search-patterns", "일치해야하는 파일 패턴 목록입니다.");
    Option<string[]> option1 = new("--exclude-patterns", "제외해야하는 파일 패턴 목록입니다.");
    Option<string?> option2 = new("--strip-path", "파일 경로에서 제거할 경로입니다.");

    var enumerateFilesCommand = new Command("enumerate-files", "파일 목록을 열거합니다.")
    {
        argument,
        argument1,
        option,
        option1,
        option2
    };

    enumerateFilesCommand.SetHandler(EnumerateFiles, argument, argument1, option, option1, option2);

    rootCommand.Add(enumerateFilesCommand);
}

return await rootCommand.InvokeAsync([.. Args]);
