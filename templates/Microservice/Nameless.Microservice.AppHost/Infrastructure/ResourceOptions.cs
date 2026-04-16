namespace Nameless.Microservice.AppHost.Infrastructure;

public abstract record ResourceOptions {
    public required string Image { get; init; }
    
    public string? RegistryUrl { get; init; }
    
    public int HostPort { get; init; }
    
    public string? VolumeName { get; init; }
    
    public bool IsPersistent { get; init; }
    
    public Dictionary<string, string?> Environment { get; init; } = [];
}