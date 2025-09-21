using System.Diagnostics.CodeAnalysis;

namespace Nameless.Lucene.Results;

/// <summary>
///     Base class for operation results.
/// </summary>
/// <param name="Error">
///     The error message, if any.
/// </param>
public abstract record Result(string? Error) {
    /// <summary>
    ///     Whether the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
    public bool Succeeded => string.IsNullOrWhiteSpace(Error);
}