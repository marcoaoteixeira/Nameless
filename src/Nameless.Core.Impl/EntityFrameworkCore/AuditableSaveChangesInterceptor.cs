using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nameless.EntityFrameworkCore.Entities;

namespace Nameless.EntityFrameworkCore;

public class AuditableSaveChangesInterceptor : SaveChangesInterceptor {
    private readonly TimeProvider _timeProvider;

    public AuditableSaveChangesInterceptor(TimeProvider timeProvider) {
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
        var dbContext = eventData.Context;
        if (dbContext is null) {
            throw new InvalidOperationException("There is no database context available at the moment.");
        }

        var entries = dbContext.ChangeTracker.Entries<IAuditable>();
        var now = _timeProvider.GetUtcNow();

        foreach (var entry in entries) {
            switch (entry.State) {
                case EntityState.Added:
                    entry.Entity.CreationDate = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModificationDate = now;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}