namespace Nameless.Results;

/// <summary>
///     Represents a result of an operation.
/// </summary>
public interface IResult {
    /// <summary>
    ///     Gets the result of the operation.
    /// </summary>
    object? Value { get; }

    /// <summary>
    ///     Gets the index of the result.
    /// </summary>
    int Index { get; }
}