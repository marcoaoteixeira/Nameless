using Microsoft.EntityFrameworkCore;

namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore;

public interface IDatabaseSeeder {
    Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken);

    void Execute(DbContext dbContext, bool storeManagementOperation);
}