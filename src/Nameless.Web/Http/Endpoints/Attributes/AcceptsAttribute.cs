namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Declares the expected request body type and media type for an endpoint.
/// </summary>
/// <remarks>
///     Only one <see cref="AcceptsAttribute"/> may be applied per endpoint
///     class. The source generator emits an
///     <c>Accepts&lt;TRequest&gt;(contentType)</c> call on the route handler
///     builder when this attribute is present.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AcceptsAttribute : Attribute {
    /// <summary>
    ///     Gets the CLR type of the expected request body.
    /// </summary>
    public Type RequestType { get; }

    /// <summary>
    ///     Gets or sets the expected media type
    ///     (e.g., <c>"application/json"</c>).
    ///     When <see langword="null"/> the framework's default content
    ///     type is used.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    ///     Initializes a new instance of <see cref="AcceptsAttribute"/>
    ///     with the specified request body type.
    /// </summary>
    /// <param name="requestType">
    ///     The CLR type of the expected request body.
    /// </param>
    public AcceptsAttribute(Type requestType) {
        RequestType = requestType;
    }
}

/// <summary>
///     Declares the expected request body type and media type for an endpoint
///     using a generic type parameter.
/// </summary>
/// <typeparam name="T">
///     The CLR type of the expected request body.
/// </typeparam>
public sealed class AcceptsAttribute<T> : AcceptsAttribute {
    /// <summary>
    ///     Initializes a new instance of <see cref="AcceptsAttribute{T}"/>
    ///     with an optional content type.
    /// </summary>
    /// <param name="contentType">
    ///     The expected media type (e.g., <c>"application/json"</c>).
    ///     When <see langword="null"/> the framework's default content
    ///     type is used.
    /// </param>
    public AcceptsAttribute(string? contentType = "application/json")
        : base(typeof(T)) { ContentType = contentType; }
}
