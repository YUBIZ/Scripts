# 개요

간단한 C# 스크립트 모음입니다.

# 요구사항

스크립트를 실행하기 위해서는 dotnet-script가 설치되어있어야 합니다.

dotnet-script 설치하기 위해서는 아래의 명령어를 실행합니다.

> dotnet tool install -g dotnet-script

# 사용법

스크립트를 실행하기 위해서는 아래의 명령어를 실행합니다.

> dotnet script Main.csx [sub-command]

## 명령어 목록

| 명령어             | 설명                                                                  |
| ------------------ | --------------------------------------------------------------------- |
| generate-file-list | source-dir의 전체 파일 목록을 output-path에 json 형식으로 출력합니다. |
| generate-file-tree | source-dir의 전체 파일 트리를 output-path에 json 형식으로 출력합니다. |

# 참고

-   [dotnet script GitHub 저장소](https://github.com/dotnet-script/dotnet-script)
-   [System.CommandLine 문서](https://learn.microsoft.com/ko-kr/dotnet/standard/commandline)
