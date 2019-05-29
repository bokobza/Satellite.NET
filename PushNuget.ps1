rm "src\Satellite.NET\bin\Debug\" -Recurse -Force
dotnet pack src\Satellite.NET --configuration Debug --include-source --include-symbols 
dotnet nuget push "src\Satellite.NET\bin\debug\*.symbols.nupkg" --source "https://api.nuget.org/v3/index.json"