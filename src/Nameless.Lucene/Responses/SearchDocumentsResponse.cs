using Nameless.Lucene.Requests;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Responses;

/// <summary>
///     Represents the <see cref="SearchDocumentsResponse{TDocument}"/> metadata.
/// </summary>
/// <typeparam name="TDocument">
///     Type of the document.
/// </typeparam>
/// <param name="Documents">
///     The documents.
/// </param>
/// <param name="TotalCount">
///     The total number of documents for the search query.
/// </param>
public record SearchDocumentsMetadata<TDocument>(TDocument[] Documents, int TotalCount)
    where TDocument : class, new() {
    /// <summary>
    ///     Gets an empty metadata.
    /// </summary>
    public static SearchDocumentsMetadata<TDocument> Empty => new(Documents: [], TotalCount: 0);
}

/// <summary>
///     Represents the response for a <see cref="SearchDocumentsRequest"/>.
/// </summary>
public class SearchDocumentsResponse<TDocument> : Result<SearchDocumentsMetadata<TDocument>>
    where TDocument : class, new() {
    private SearchDocumentsResponse(SearchDocumentsMetadata<TDocument>? value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="SearchDocumentsMetadata{TDocument}"/> into a
    ///     <see cref="SearchDocumentsResponse{TDocument}"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="SearchDocumentsMetadata{TDocument}"/> instance.
    /// </param>
    public static implicit operator SearchDocumentsResponse<TDocument>(SearchDocumentsMetadata<TDocument> value) {
        return new SearchDocumentsResponse<TDocument>(value, errors: []);
    }

    /// <summary>
    ///     Converts an array of <see cref="Error"/> into a
    ///     <see cref="SearchDocumentsResponse{TDocument}"/>.
    /// </summary>
    /// <param name="errors">
    ///     The array of <see cref="Error"/> instance.
    /// </param>
    public static implicit operator SearchDocumentsResponse<TDocument>(Error[] errors) {
        return new SearchDocumentsResponse<TDocument>(value: null, errors);
    }

    /// <summary>
    ///     Converts a <see cref="Error"/> into a
    ///     <see cref="SearchDocumentsResponse{TDocument}"/>.
    /// </summary>
    /// <param name="error">
    ///     The <see cref="Error"/> instance.
    /// </param>
    public static implicit operator SearchDocumentsResponse<TDocument>(Error error) {
        return new SearchDocumentsResponse<TDocument>(value: null, errors: [error]);
    }

    internal static Task<SearchDocumentsResponse<TDocument>> From(TDocument[] documents, int totalCount) {
        return Task.FromResult<SearchDocumentsResponse<TDocument>>(
            new SearchDocumentsMetadata<TDocument>(documents, totalCount)
        );
    }

    internal static Task<SearchDocumentsResponse<TDocument>> From(params Error[] errors) {
        return Task.FromResult<SearchDocumentsResponse<TDocument>>(errors);
    }
}