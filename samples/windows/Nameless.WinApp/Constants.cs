namespace Nameless.WinApp;

internal static class Constants {
    internal static class Labels {
        internal static class NavigationMenuItems {
            internal const string Configuration = "Configurações";
        }
    }

    internal static class Database {
        internal const string FileName = "app.db";
        internal const string ConnStrPattern = "Data Source={0};Pooling=false;";
    }

    internal static class Logging {
        internal const string Directory = "logs";
        internal const string FileNamePattern = "app-.log";
    }

    internal static class Application {
        internal const string Name = "Windows Application";
    }
}
