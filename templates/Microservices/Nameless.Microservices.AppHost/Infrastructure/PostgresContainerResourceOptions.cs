namespace Nameless.Microservices.AppHost.Infrastructure;

public sealed record PostgresContainerResourceOptions : ContainerResourceOptions {
    public PostgresAdministrationContainerResourceOptions? PgAdmin { get; init; }
}

public sealed record PostgresAdministrationContainerResourceOptions : ContainerResourceOptions;