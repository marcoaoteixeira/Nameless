namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

/// <summary>
///     Marks an endpoint handler class as deprecated, signalling that it
///     will be removed in a future version and should no longer be used
///     in new integrations.
/// </summary>
/// <remarks>
///     When applied, this attribute is intended to be picked up by
///     middleware or OpenAPI configuration to automatically set
///     <c>op.Deprecated = true</c> on the corresponding operation, inject
///     the <c>Sunset</c> HTTP response header (per
///     <see href="https://www.rfc-editor.org/rfc/rfc8594">RFC 8594</see>),
///     and surface the deprecation <see cref="Message"/> in the API
///     documentation.
/// </remarks>
/// <example>
/// <code>
/// <![CDATA[
///     [Endpoint<Get>(Version = "1")]
///     [Deprecate(
///         Message = "Use /v2/users instead.",
///         Sunset  = "Wed, 31 Dec 2025 00:00:00 GMT")]
///     public partial class GetUsersV1Endpoint { ... }
/// ]]>
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DeprecateAttribute : Attribute {
    /// <summary>
    ///     Gets or sets a human-readable message describing the deprecation,
    ///     typically indicating a migration path or a successor endpoint.
    /// </summary>
    /// <value>
    ///     A plain-text string shown to consumers via API documentation or
    ///     response headers, or <see langword="null"/> if no message is
    ///     provided.
    /// </value>
    /// <example>
    /// <code>
    ///     [Deprecate(Message = "Use /v2/users instead.")]
    /// </code>
    /// </example>
    public string? Message { get; init; }

    /// <summary>
    ///     Gets or sets the date after which the endpoint will no longer be
    ///     available, expressed as an HTTP-date string per
    ///     <see href="https://www.rfc-editor.org/rfc/rfc8594">RFC 8594</see>.
    /// </summary>
    /// <value>
    ///     An HTTP-date string (e.g. <c>"Wed, 31 Dec 2025 00:00:00 GMT"</c>)
    ///     used as the value of the <c>Sunset</c> response header, or
    ///     <see langword="null"/> if no sunset date has been defined for this
    ///     endpoint.
    /// </value>
    /// <remarks>
    ///     The value must be a valid HTTP-date as defined by
    ///     <see href="https://www.rfc-editor.org/rfc/rfc7231#section-7.1.1.1">
    ///     RFC-7231 §7.1.1.1</see> (IMF-fixdate format). Malformed values may
    ///     be silently ignored or cause the middleware to skip injecting the
    ///     header — validate the format before setting it.
    /// </remarks>
    /// <example>
    /// <code>
    ///     [Deprecate(Sunset = "Wed, 31 Dec 2025 00:00:00 GMT")]
    /// </code>
    /// </example>
    public string? Sunset { get; init; }

    /// <summary>
    ///     Gets the link pointing to migration documentation.
    /// </summary>
    public string? Link { get; init; }
}
