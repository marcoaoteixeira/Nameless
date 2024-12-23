using System.Collections.ObjectModel;

namespace Nameless.Result;

public sealed record Error {
    public string Description { get; }

    public ErrorType Type { get; }

    public ReadOnlyDictionary<string, object> Metadata { get; }

    private Error(string description, ErrorType type, ReadOnlyDictionary<string, object> metadata) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);
        Type = type;
        Metadata = metadata;
    }

    public static Error Validation(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Validation, metadata ?? []);

    public static Error Missing(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Missing, metadata ?? []);

    public static Error Conflict(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Conflict, metadata ?? []);

    public static Error Failure(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Failure, metadata ?? []);

    public static Error Forbidden(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Forbidden, metadata ?? []);

    public static Error Unauthorized(string description, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Unauthorized, metadata ?? []);

    private static Error Create(string description, ErrorType type, Dictionary<string, object> metadata)
        => new(description, type, metadata: new ReadOnlyDictionary<string, object>(metadata));
}