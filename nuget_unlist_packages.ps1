$command = "dotnet nuget delete $package $version -k oy2p7si34f5upmxensta3azpdwy7lckuxnudwkhjkarfoy -s https://api.nuget.org/v3/index.json --non-interactive"

Get-ChildItem -Path .\src\ -Filter *.csproj -Recurse -Exclude *.UnitTests.csproj,*'- Backup.'* | ForEach-Object -Process { $_.BaseName }