global using Nameless.Testing.Tools.Attributes;
global using SysPath = System.IO.Path;
global using SysFile = System.IO.File;
global using SysDirectory = System.IO.Directory;

namespace Nameless.WinApp.Tests;

// Microsoft.EntityFrameworkCore.Sqlite.Core and Microsoft.Data.Sqlite.Core are
// provider-agnostic; without this the first raw SQLite call throws "You need
// to call SQLitePCL.raw.SetProvider()".
internal static class SqliteProviderInitializer {
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Initialize() {
        SQLitePCL.Batteries_V2.Init();
    }
}
