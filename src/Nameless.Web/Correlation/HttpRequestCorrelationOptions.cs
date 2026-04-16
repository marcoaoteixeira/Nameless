using Nameless.Attributes;

namespace Nameless.Web.Correlation;

[ConfigurationSectionName("HttpRequestCorrelation")]
public class HttpRequestCorrelationOptions {
    /// <summary>
    ///     Gets or sets the key used to store the correlation ID.
    /// </summary>
    public string HeaderKey { get; set; } = HttpRequestCorrelationDefaults.HeaderKey;

    /// <summary>
    ///     Whether it should include the correlation ID value in the response
    ///     for client side tracking.
    /// </summary>
    public bool IncludeInResponse { get; set; } = true;
}