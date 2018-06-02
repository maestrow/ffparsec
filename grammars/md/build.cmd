dotnet clean .\src\Markdown.fsproj
dotnet restore .\src\Markdown.fsproj
dotnet build .\src\Markdown.fsproj

dotnet clean .\tests\MdTests.fsproj
dotnet restore .\tests\MdTests.fsproj
dotnet build .\tests\MdTests.fsproj