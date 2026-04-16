namespace Nameless.Microservices.Infrastructure.Scalar.Impl;

public abstract record AuthorizationSchemeOptions {
    public required string Issuer { get; set; }
}