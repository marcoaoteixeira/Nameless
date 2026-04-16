using System.Data;

namespace Nameless.Data.Requests;

public sealed record ExecuteReaderRequest<TResult> : RequestBase {
    public required Func<IDataRecord, TResult> Mapper { get; init; }
}