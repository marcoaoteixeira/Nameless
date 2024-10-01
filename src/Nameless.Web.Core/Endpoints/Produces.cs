using System.Net;

namespace Nameless.Web.Endpoints;

/// <summary>
/// Represents the endpoint produces result information.
/// </summary>
public sealed record Produces {
    private const string APPLICATION_JSON = "application/json";
    private const string APPLICATION_PROBLEM_JSON = "application/problem+json";

    /// <summary>
    /// Gets or init additional content types.
    /// </summary>
    public string[] AdditionalContentTypes { get; init; } = [];

    /// <summary>
    /// Gets or init the content type.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Gets or init the response type.
    /// </summary>
    public Type ResponseType { get; init; } = typeof(void);

    /// <summary>
    /// Gets or init the HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    /// <summary>
    /// Gets or init the produce type (Default, Problem or ValidationProblem).
    /// </summary>
    public ProducesType Type { get; init; }

    /// <summary>
    /// Creates a <see cref="Produces"/> for simple results.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type.</param>
    /// <returns>
    /// A <see cref="Produces"/> for simple results.
    /// </returns>
    public static Produces Result<TResponse>(HttpStatusCode statusCode = HttpStatusCode.OK,
                                             string contentType = APPLICATION_JSON)
        => Result(typeof(TResponse), statusCode, contentType);

    /// <summary>
    /// Creates a <see cref="Produces"/> for simple results.
    /// </summary>
    /// <param name="responseType">Type of the response.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type.</param>
    /// <returns>
    /// A <see cref="Produces"/> for simple results.
    /// </returns>
    public static Produces Result(Type responseType,
                                  HttpStatusCode statusCode = HttpStatusCode.OK,
                                  string contentType = APPLICATION_JSON)
        => new() {
            ResponseType = Prevent.Argument.Null(responseType),
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };

    /// <summary>
    /// Creates a <see cref="Produces"/> for problem results.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type.</param>
    /// <returns>
    /// A <see cref="Produces"/> for problem results.
    /// </returns>
    public static Produces Problem(HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
                                   string contentType = APPLICATION_PROBLEM_JSON)
        => new() {
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };

    /// <summary>
    /// Creates a <see cref="Produces"/> for validation problem results.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type.</param>
    /// <returns>
    /// A <see cref="Produces"/> for validation problem results.
    /// </returns>
    public static Produces ValidationProblem(HttpStatusCode statusCode = HttpStatusCode.BadRequest,
                                             string contentType = APPLICATION_PROBLEM_JSON)
        => new() {
            StatusCode = statusCode,
            ContentType = Prevent.Argument.Null(contentType)
        };
}