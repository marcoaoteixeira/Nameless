using Microsoft.EntityFrameworkCore;
using Nameless.Registration;

namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore;

[AssemblyScan(Ignore = true)]
public sealed class NullDatabaseSeeder : IDatabaseSeeder {
    public static IDatabaseSeeder Instance { get; } = new NullDatabaseSeeder();

    static NullDatabaseSeeder() { }

    private NullDatabaseSeeder() { }
    public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    public void Execute(DbContext dbContext, bool storeManagementOperation) {
        /* do nothing */
    }
}