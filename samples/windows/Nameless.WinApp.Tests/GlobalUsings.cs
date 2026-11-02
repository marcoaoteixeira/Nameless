global using Nameless.Testing.Tools.Attributes;
global using SysPath = System.IO.Path;
global using SysFile = System.IO.File;
global using SysDirectory = System.IO.Directory;

// SQLite provider registration (SQLitePCL.Batteries_V2.Init()) now happens
// via Nameless.WinApp's own module initializer (see its GlobalUsings.cs),
// which runs as soon as this test assembly touches a Nameless.WinApp type
// (e.g. PerformApplicationBackupRequestHandler) — before any SQLite API use.
