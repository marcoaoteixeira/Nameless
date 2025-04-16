namespace Nameless.Results;

/// <summary>
/// Represents a result of an operation.
/// </summary>
public interface IResult {
    object? Value { get; }
    int Index { get; }
}