namespace Nameless.Microservices.AppHost.Options;

public record PostgresResourceOptions : ContainerResourceOptions {
    public string? ConnectionStringName { get; set; }

    public string? DatabaseName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public Action<PostgresAdministratorResourceOptions>? UsePgAdmin { get; set; }
}