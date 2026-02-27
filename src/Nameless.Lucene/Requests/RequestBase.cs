namespace Nameless.Lucene.Requests;

/// <summary>
///     Represents a base class for a Lucene index interaction request.
/// </summary>
/// <param name="IndexName">
///     The index name.
/// </param>
public record RequestBase(string? IndexName);