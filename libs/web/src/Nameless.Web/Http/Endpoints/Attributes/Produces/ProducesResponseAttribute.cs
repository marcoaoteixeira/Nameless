namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a successful response type and HTTP status code for an
///     endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesResponseAttribute"/> instances may
///     be applied to the same endpoint class to document several possible
///     success responses. The source generator emits a corresponding
///     <c>Produces&lt;TResponse&gt;(statusCode, contentType)</c> call
///     on the route handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ProducesResponseAttribute : Attribute {
    /// <summary>
    ///     Gets the CLR type of the response body.
    /// </summary>
    public Type ResponseType { get; }

    /// <summary>
    ///     Gets the HTTP status code returned with this response
    ///     (e.g., <c>200</c>, <c>201</c>).
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    ///     Gets or sets the media type of the response body
    ///     (e.g., <c>"application/json"</c>). If <see langword="null"/> the
    ///     framework's default content type is used.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    ///     Gets or sets additional media types of the response body
    ///     (e.g., <c>"text/plain"</c>).
    /// </summary>
    public string[] AdditionalContentTypes { get; init; } = [];

    /// <summary>
    ///     Initializes a new instance of <see cref="ProducesResponseAttribute"/>
    ///     with the specified response type and status code.
    /// </summary>
    /// <param name="responseType">
    ///     The CLR type of the response body.
    /// </param>
    public ProducesResponseAttribute(Type responseType) {
        ResponseType = responseType;
    }
}

/// <summary>
///     Declares a successful response type and HTTP status code for an
///     endpoint using a generic type parameter.
/// </summary>
/// <typeparam name="T">
///     The CLR type of the response body.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ProducesResponseAttribute<T> : ProducesResponseAttribute {
    /// <summary>
    ///     Initializes a new instance of <see cref="ProducesResponseAttribute{T}"/>
    ///     with the specified status code and optional content type.
    /// </summary>
    public ProducesResponseAttribute() : base(typeof(T)) { }
}
