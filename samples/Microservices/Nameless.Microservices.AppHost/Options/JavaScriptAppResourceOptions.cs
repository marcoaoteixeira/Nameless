namespace Nameless.Microservices.AppHost.Options;

public record JavaScriptAppResourceOptions : WebResourceOptions {
    public string? RunScriptName { get; set; }
    public bool UseSsl { get; set; }
    public int Port { get; set; }
}