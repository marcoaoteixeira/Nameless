namespace Nameless.Windows.DisasterRecovery;

public record BackupInput {
    public required string SourceDirectoryPath { get; set; }
    public required string DestinationDirectoryPath { get; set; }
}