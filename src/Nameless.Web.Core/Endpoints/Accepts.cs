namespace Nameless.Web.Endpoints;
/// <summary>
/// Creates a class with information about Accept request HTTP header.
/// The Accept request HTTP header indicates which content types, expressed
/// as MIME types, the client is able to understand.
/// </summary>
public sealed record Accepts {
    /// <summary>
    /// Gets or init additional content types.
    /// </summary>
    public string[] AdditionalContentTypes { get; init; } = [];
    /// <summary>
    /// Gets or init the default content type.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;
    /// <summary>
    /// Whether this Accepts information is optional.
    /// </summary>
    public bool IsOptional { get; init; }
    /// <summary>
    /// Gets or init the request type.
    /// </summary>
    public Type RequestType { get; init; } = typeof(void);

    /// <summary>
    /// Creates an Accepts information for the specified type.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <param name="contentType">The content type.</param>
    /// <param name="additionalContentTypes">The additional content types.</param>
    /// <returns>
    /// An instance of <see cref="Accepts"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="contentType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="contentType"/> is empty or white spaces.
    /// </exception>
    public static Accepts For<TRequest>(string contentType, params string[] additionalContentTypes)
        => For<TRequest>(contentType, isOptional: false, additionalContentTypes);

    /// <summary>
    /// Creates an Accepts information for the specified type.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request.</typeparam>
    /// <param name="contentType">The content type.</param>
    /// <param name="isOptional">Whether is optional or not.</param>
    /// <param name="additionalContentTypes">The additional content types.</param>
    /// <returns>
    /// An instance of <see cref="Accepts"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="contentType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="contentType"/> is empty or white spaces.
    /// </exception>
    public static Accepts For<TRequest>(string contentType, bool isOptional, params string[] additionalContentTypes)
        => new() {
            RequestType = typeof(TRequest),
            ContentType = Prevent.Argument.NullOrWhiteSpace(contentType),
            AdditionalContentTypes = additionalContentTypes,
            IsOptional = isOptional
        };
}
