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

    internal static class BackupRecovery {
        internal const string FileNamePattern = "backup-{0:yyyyMMddHHmmss}.zip";

        // Duplicated from AppConfigurationManager's private APP_CONFIGURATION_FILE
        // constant — acceptable for a sample app; revisit if a third consumer
        // of the app.config file name appears.
        internal const string AppConfigFileName = "app.config";
    }

    internal static class Logging {
        internal const string Directory = "logs";
        internal const string FileNamePattern = "app-.log";
    }

    internal static class Application {
        internal const string Name = "Windows Application";
    }
}
