namespace Nameless.WPF;

public static class WPFConstants {
    public const string BackupFileNamePattern = "{0:yyyyMMddHHmmss}.bkapp";
    public const string BackupFileExtension = ".bkapp";

    public static class FolderStructure {
        public const string BackupDirectoryName = "backups";
        public const string DatabaseDirectoryName = "databases";
        public const string TemporaryDirectoryName = "tmp";
        public const string UpdateDirectoryName = "updates";
    }

    public static class BackupDefinitions {
        public const string FileNamePattern = "{0:yyyyMMddHHmmss}{1}";

        public const string ApplicationExtension = ".bkapp";
        public const string SqliteExtension = ".dat";
        public const string LuceneExtension = ".idx";
    }

    public static class Lucene {
        public const string UniqueIndexName = "78213cbeef85474686c80570279befd5";
    }
}
