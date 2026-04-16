namespace Nameless.Microservice.AppHost.Infrastructure;

public sealed record PostgresResourceOptions : ResourceOptions {
    public PostgresAdministrationResourceOptions? PgAdmin { get; init; }
}

public sealed record PostgresAdministrationResourceOptions : ResourceOptions;