using Nameless.Attributes;

namespace Nameless.Auth.OAuth;

[ConfigurationSectionName("OAuth")]
public record OAuthOptions {
    public string AuthorityUrl { get; init; } = string.Empty;

    public Dictionary<string, string?> Header { get; init; } = [];
}
