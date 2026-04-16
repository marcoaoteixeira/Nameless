namespace Nameless.WPF.DisasterRecovery;

public record RestoreInput {
    public required string TemporaryDirectoryPath { get; init; }
}