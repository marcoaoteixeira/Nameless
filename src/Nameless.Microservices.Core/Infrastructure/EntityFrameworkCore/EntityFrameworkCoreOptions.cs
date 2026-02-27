namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore;

public class EntityFrameworkCoreOptions {
    public string ConnectionStringName { get; set; } = "Default";

    public EntityFrameworkCoreConnector Connector { get; set; }
}

public enum EntityFrameworkCoreConnector {
    Sqlite,

    PostgreSQL,
}