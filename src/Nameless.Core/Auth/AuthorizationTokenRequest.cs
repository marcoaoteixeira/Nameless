namespace Nameless.Auth;

public record AuthorizationTokenRequest {
    public string? Scheme { get; init; }
}