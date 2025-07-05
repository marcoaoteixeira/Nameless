namespace Nameless.Web.Correlation;

public record CorrelationOptions {
    /// <summary>
    ///     Gets or sets the key used to store the correlation ID.
    /// </summary>
    public string Key { get; set; } = "X-Correlation-ID";

    /// <summary>
    ///     Whether to use the HTTP header for correlation ID.
    /// </summary>
    /// <remarks>
    ///     Default is <see langword="true"/>, meaning the correlation ID will
    ///     be read from the HTTP header or write to it.
    ///     If set to <see langword="false"/>, the correlation ID will be stored
    ///     in the HTTP context items instead.
    /// </remarks>
    public bool UseHeader { get; set; } = true;
}