dotnet clean .\src\Parsec.fsproj
dotnet clean .\tests\Tests.fsproj
dotnet restore .\src\Parsec.fsproj
dotnet build .\src\Parsec.fsproj
dotnet restore .\tests\Tests.fsproj
dotnet build .\tests\Tests.fsproj