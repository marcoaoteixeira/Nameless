using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore;

public interface IDatabaseSeeder {
    Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken);

    void Execute(DbContext dbContext, bool storeManagementOperation);
}