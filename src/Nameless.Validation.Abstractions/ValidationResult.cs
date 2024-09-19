namespace Nameless.Validation.Abstractions;

public sealed record ValidationResult {
    public static ValidationResult Empty => new();

    public ValidationEntry[] Errors { get; } = [];

    public bool Succeeded => Errors.Length == 0;

    public ValidationResult(params ValidationEntry[] errors) {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }
}