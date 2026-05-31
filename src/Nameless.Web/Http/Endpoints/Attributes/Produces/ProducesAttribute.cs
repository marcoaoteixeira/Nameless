namespace Nameless.Web.Http.Endpoints.Attributes.Produces;

/// <summary>
///     Declares a successful response type and HTTP status code for an
///     endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="ProducesAttribute"/> instances may be applied
///     to the same endpoint class to document several possible success
///     responses. The source generator emits a corresponding
///     <c>Produces&lt;TResponse&gt;(statusCode, contentType)</c> call
///     on the route handler builder.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ProducesAttribute : Attribute {
    /// <summary>
    ///     Gets the CLR type of the response body.
    /// </summary>
    public Type ResponseType { get; }

    /// <summary>
    ///     Gets the HTTP status code returned with this response
    ///     (e.g., <c>200</c>, <c>201</c>).
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Gets or sets the media type of the response body
    ///     (e.g., <c>"application/json"</c>). When <see langword="null"/>
    ///     the framework's default content type is used.
    /// </summary>
    public string? ContentType { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="ProducesAttribute"/>
    ///     with the specified response type and status code.
    /// </summary>
    /// <param name="responseType">
    ///     The CLR type of the response body.
    /// </param>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>200</c>.
    /// </param>
    /// <param name="contentType">
    ///     The content type.
    /// </param>
    public ProducesAttribute(Type responseType, int statusCode = 200, string? contentType = Constants.ContentType) {
        ResponseType = responseType;
        StatusCode = statusCode;
        ContentType = contentType ?? Constants.ContentType;
    }
}

/// <summary>
///     Declares a successful response type and HTTP status code for an
///     endpoint using a generic type parameter.
/// </summary>
/// <typeparam name="T">
///     The CLR type of the response body.
/// </typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class ProducesAttribute<T> : ProducesAttribute {
    /// <summary>
    ///     Initializes a new instance of <see cref="ProducesAttribute{T}"/>
    ///     with the specified status code and optional content type.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code. Defaults to <c>200</c>.
    /// </param>
    /// <param name="contentType">
    ///     The media type of the response body. When <see langword="null"/>
    ///     the framework default is used.
    /// </param>
    public ProducesAttribute(int statusCode = 200, string? contentType = Constants.ContentType)
        : base(typeof(T), statusCode, contentType) { }
}
