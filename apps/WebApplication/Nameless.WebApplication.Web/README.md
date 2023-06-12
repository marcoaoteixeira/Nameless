# Nameless Web Application

This repository contains all code that I assume is useful in most
of the cases where I need to build something. So, if you think
that it could be useful for you, feel free to fork, clone, etc.
Also, I tried to mention every person that I might get something
from. If you find code that needs to be given the correct authorship,
please, let me know.
<br><br>

## Workflow Status
<br>

![Main branch badge](https://github.com/marcoaoteixeira/WebApplication_Server/actions/workflows/main.yml/badge.svg)

![Development branch badge](https://github.com/marcoaoteixeira/WebApplication_Server/actions/workflows/develop.yml/badge.svg)
## Starting

Instructions below will show your the way to get things working.
<br><br>

## Pre-requirements

If working with Docker and local SQL Server instance, enable SQL Server to listen TCP/IP.
See: [Configure a Server to Listen on a Specific TCP Port](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/configure-a-server-to-listen-on-a-specific-tcp-port?view=sql-server-ver16)
<br><br>

## EntityFramework Core

### EF Tools
```powershell
dotnet tool install --global dotnet-ef
```

There're two *powershell* scripts in the root folder that are used to update and migrate the database.

```powershell
# Updates the database.
db_update.ps1
```

```powershell
# Creates a new migration when entity objects changes.
# Script will ask for a new migration name before execution.
db_migration.ps1
```

### Further Reading

[Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)

[Entity Framework Tutorials](https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx)
<br><br>

## Testing

There are some test projects inside the "test" folder. Maybe you'll need to
install the coverage tool and a report tool

*.NET Coverlet Tool*
```powershell
dotnet tool install -g coverlet.console
```

*.NET Report Generator Tool*
```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
```
<br>

## Coding Styles

Nothing defined, use your good sense.
<br><br>

## Deployment

This particular project uses GitHub Workflows as its CI. All files are
located in the .github folder.
<br><br>

## Contribuition

Just me, at the moment.
<br><br>

## Versioning

Using [SemVer](http://semver.org/) for assembly versioning.
<br><br>

## Authors

* **Marco Teixeira (marcoaoteixeira)** - *initial work*
<br><br>

## License

[MIT](https://opensource.org/licenses/MIT)
<br><br>

## Acknowledgement

* Hat tip to anyone whose code was used.