using System.Diagnostics.CodeAnalysis;
using Nameless.Attributes;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Auth.OAuth;

/// <summary>
///     Represents the OAuth configuration options.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.PlainCodeStructure)]
[ConfigurationSectionName("OAuth")]
public record OAuthOptions {
    /// <summary>
    ///     Gets the authority URL.
    /// </summary>
    public string? AuthorityUrl { get; init; }

    /// <summary>
    ///     Gets the token endpoint.
    /// </summary>
    public string? TokenEndpoint { get; init; } = "/oauth/token";
    
    /// <summary>
    ///     Gets the timeout to request an authorization token.
    /// </summary>
    /// <remarks>
    ///     The value <![CDATA[-1]]> indicate to wait indefinitely.
    /// </remarks>
    public int Timeout { get; init; } = -1;
}
