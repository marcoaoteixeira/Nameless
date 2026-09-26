using System.Data;

namespace Nameless.Data.Requests;

/// <summary>
///     Represents an execute reader request.
/// </summary>
public sealed record ExecuteReaderRequest<TResult> : RequestBase {
    /// <summary>
    ///     Gets the mapper for the record.
    /// </summary>
    public required Func<IDataRecord, TResult> Mapper { get; init; }
}