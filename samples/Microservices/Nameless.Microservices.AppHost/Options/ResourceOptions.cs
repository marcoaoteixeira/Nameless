namespace Nameless.Microservices.AppHost.Options;

public abstract record ResourceOptions {
    public string? Name { get; set; }

    public int Replicas { get; set; }
}