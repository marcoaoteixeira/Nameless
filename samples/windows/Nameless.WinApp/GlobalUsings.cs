global using SysPath = System.IO.Path;
global using SysDirectory = System.IO.Directory;
global using SysFile = System.IO.File;

namespace Nameless.WinApp;

// Microsoft.EntityFrameworkCore.Sqlite.Core / Microsoft.Data.Sqlite.Core are
// provider-agnostic; without this the first SQLite call throws "You need to
// call SQLitePCL.raw.SetProvider()". A module initializer guarantees this
// runs before anything (including the static WinHost field below in App)
// touches SQLite, regardless of initialization order.
internal static class SqliteProviderInitializer {
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Initialize() {
        SQLitePCL.Batteries_V2.Init();
    }
}