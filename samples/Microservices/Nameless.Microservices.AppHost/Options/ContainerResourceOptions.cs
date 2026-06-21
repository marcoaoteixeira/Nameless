namespace Nameless.Microservices.AppHost.Options;

public abstract record ContainerResourceOptions : ResourceOptions {
    public string? RegistryUrl { get; set; }

    public string? Image { get; set; }

    public int HostPort { get; set; }

    public string? VolumeName { get; set; }

    public bool IsPersistent { get; set; }

    public Dictionary<string, string?> Environment { get; set; } = [];
}