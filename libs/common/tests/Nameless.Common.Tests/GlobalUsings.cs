global using Nameless.Testing.Tools.Attributes;
global using SysPath = System.IO.Path;
global using SysFile = System.IO.File;
global using SysDirectory = System.IO.Directory;
using System.Runtime.CompilerServices;
using SQLitePCL;

namespace Nameless;

// Nameless.Common references Microsoft.EntityFrameworkCore.Sqlite.Core,
// which is provider-agnostic: without this the first SQLite call throws
// "You need to call SQLitePCL.raw.SetProvider()". A module initializer
// guarantees this runs before any test touches SQLite, regardless of
// test execution order.
internal static class SqliteProviderInitializer {
    [ModuleInitializer]
    public static void Initialize() {
        Batteries_V2.Init();
    }
}