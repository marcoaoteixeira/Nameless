namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Declares a <c>ValidationProblemDetails</c> response for an endpoint,
///     typically used to document a <c>400 Bad Request</c> caused by
///     invalid input.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesValidationProblemAttribute"/> instances
///     may be applied to the same endpoint class. The source generator
///     emits a corresponding
///     <c>ProducesValidationProblem(statusCode, contentType)</c> call on
///     the route handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class ProducesValidationProblemAttribute : Attribute {
    /// <summary>
    ///     Gets the HTTP status code for this validation error response.
    ///     Defaults to <c>400</c>.
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
    ///     <see cref="ProducesValidationProblemAttribute"/> with the specified
    ///     status code.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>400</c>.
    /// </param>
    public ProducesValidationProblemAttribute(int statusCode = 400) {
        StatusCode = statusCode;
    }
}
