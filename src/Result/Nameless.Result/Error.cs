using System.Collections.ObjectModel;

namespace Nameless.Results;

public sealed record Error {
    public string Description { get; }

    public ErrorType Type { get; }

    public Exception? Exception { get; }

    public ReadOnlyDictionary<string, object> Metadata { get; }

    private Error(string description, ErrorType type, Exception? exception, ReadOnlyDictionary<string, object> metadata) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);
        Type = type;
        Exception = exception;
        Metadata = metadata;
    }

    public static Error Validation(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Validation, exception, metadata ?? []);

    public static Error Missing(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Missing, exception, metadata ?? []);

    public static Error Conflict(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Conflict, exception, metadata ?? []);

    public static Error Failure(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Failure, exception, metadata ?? []);

    public static Error Forbidden(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Forbidden, exception, metadata ?? []);

    public static Error Unauthorized(string description, Exception? exception = null, Dictionary<string, object>? metadata = null)
        => Create(description, ErrorType.Unauthorized, exception, metadata ?? []);

    private static Error Create(string description, ErrorType type, Exception? exception, Dictionary<string, object> metadata)
        => new(description, type, exception, metadata: new ReadOnlyDictionary<string, object>(metadata));
}