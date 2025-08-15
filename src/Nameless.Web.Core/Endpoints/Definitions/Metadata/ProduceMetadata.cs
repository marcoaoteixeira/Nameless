namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Represents metadata for the response produced by an endpoint.
/// </summary>
public record ProduceMetadata {
    /// <summary>
    ///     Gets the response type that the endpoint produces.
    /// </summary>
    public Type ResponseType { get; }

    /// <summary>
    ///     Gets the HTTP status code returned by the endpoint.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Gets the content types that the endpoint produces.
    /// </summary>
    public string[] ContentTypes { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ProduceMetadata"/> class.
    /// </summary>
    /// <param name="responseType">
    ///     The response type that the endpoint produces.
    /// </param>
    /// <param name="statusCode">
    ///     The HTTP status code returned by the endpoint.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint produces.
    /// </param>
    public ProduceMetadata(Type responseType, int statusCode, string[] contentTypes) {
        ResponseType = Guard.Against.Null(responseType);
        StatusCode = Guard.Against.OutOfRange(statusCode, 100, 999);
        ContentTypes = Guard.Against.Null(contentTypes);
    }
}