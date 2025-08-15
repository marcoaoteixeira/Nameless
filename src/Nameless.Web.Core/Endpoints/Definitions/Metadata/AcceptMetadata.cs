namespace Nameless.Web.Endpoints.Definitions.Metadata;

/// <summary>
///     Defines metadata for request acceptance in an endpoint.
/// </summary>
public record AcceptMetadata {
    /// <summary>
    ///     Gets the request type that the endpoint accepts.
    /// </summary>
    public Type RequestType { get; }

    /// <summary>
    ///     Whether the request is optional.
    /// </summary>
    public bool IsOptional { get; }

    /// <summary>
    ///     Gets the content types that the endpoint accepts.
    /// </summary>
    public string[] ContentTypes { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AcceptMetadata"/> class.
    /// </summary>
    /// <param name="requestType">
    ///     The request type.
    /// </param>
    /// <param name="isOptional">
    ///     Whether the request is optional.
    /// </param>
    /// <param name="contentTypes">
    ///     The content types that the endpoint accepts.
    /// </param>
    public AcceptMetadata(Type requestType, bool isOptional, string[] contentTypes) {
        RequestType = Guard.Against.Null(requestType);
        IsOptional = isOptional;
        ContentTypes = Guard.Against.Null(contentTypes);
    }
}