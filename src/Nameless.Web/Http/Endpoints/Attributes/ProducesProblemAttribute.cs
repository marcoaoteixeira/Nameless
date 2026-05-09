namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Declares a <c>ProblemDetails</c> error response for an endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesProblemAttribute"/> instances may
///     be applied to the same endpoint class to document several possible
///     error responses. The source generator emits a corresponding
///     <c>ProducesProblem(statusCode, contentType)</c> call on the route
///     handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ProducesProblemAttribute : Attribute {
    /// <summary>
    ///     Gets the HTTP status code for this error response
    ///     (e.g., <c>404</c>, <c>500</c>).
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Gets or sets the media type of the response body
    ///     (e.g., <c>"application/problem+json"</c>). When
    ///     <see langword="null"/> the framework's default content type is used.
    /// </summary>
    public string? ContentType { get; init; } = "application/problem+json";

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ProducesProblemAttribute"/> with the specified status
    ///     code.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>500</c>.
    /// </param>
    public ProducesProblemAttribute(int statusCode = 500) {
        StatusCode = statusCode;
    }
}
