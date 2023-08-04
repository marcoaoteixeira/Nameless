dotnet new sln -n Nameless
Get-ChildItem -Filter *.csproj -Recurse | ForEach-Object { dotnet sln Nameless.sln add $_.FullName }