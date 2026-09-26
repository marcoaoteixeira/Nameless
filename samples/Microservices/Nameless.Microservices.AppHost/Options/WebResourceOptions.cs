namespace Nameless.Microservices.AppHost.Options;

public abstract record WebResourceOptions : ResourceOptions {
    public string? HealthCheckUrl { get; set; }
}