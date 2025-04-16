namespace Nameless.Results;

public readonly struct Error {
    public string Description { get; }
    public ErrorType Type { get; }

    /// <summary>
    /// Do not use type constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// if parameterless constructor is called.
    /// </exception>
    public Error() {
        throw new InvalidOperationException("Do not use type constructor.");
    }

    private Error(string description, ErrorType type) {
        Description = Prevent.Argument.NullOrWhiteSpace(description);
        Type = type;
    }

    public static Error Validation(string description)
        => new(description, ErrorType.Validation);

    public static Error Missing(string description)
        => new(description, ErrorType.Missing);

    public static Error Conflict(string description)
        => new(description, ErrorType.Conflict);

    public static Error Failure(string description)
        => new(description, ErrorType.Failure);

    public static Error Forbidden(string description)
        => new(description, ErrorType.Forbidden);

    public static Error Unauthorized(string description)
        => new(description, ErrorType.Unauthorized);
}