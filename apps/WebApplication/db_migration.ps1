$migration_name = Read-Host -Prompt 'Migration name'
dotnet ef migrations add $migration_name -s .\Nameless.WebApplication.Web\Nameless.WebApplication.Web.csproj -p .\Nameless.WebApplication.Core\Nameless.WebApplication.Core.csproj -o Entities\Migrations