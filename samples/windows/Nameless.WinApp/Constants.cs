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

    internal static class SystemUpdate {
        internal const string RepositoryOwner = "marcoaoteixeira";
        internal const string RepositoryName = "Nameless";
        internal const string TagPrefix = "winapp-";
        internal const string RuntimeIdentifier = "win-x64";
        internal const string PackageNamePattern = "Nameless.WinApp-{0}-win-x64.zip";
        internal const string ChecksumSuffix = ".sha256";
        internal const string ExecutableName = "Nameless.WinApp.exe";
        internal const long MaxPackageBytes = 512L * 1024 * 1024;
        internal const string OldSuffixPattern = "{0}.old-{1:yyyyMMddHHmmss}";
        internal const string StagingDirectoryPattern = ".update-{0:yyyyMMddHHmmss}";

        // Forbidden root entries inside the downloaded package (release
        // contract §2.1): runtime data folders and the updater's own
        // working names. Compared ordinally, case-insensitive.
        internal static readonly string[] ForbiddenRootEntries = ["logs", "App_Data"];
    }

    internal static class Application {
        internal const string Name = "Windows Application";
    }
}
