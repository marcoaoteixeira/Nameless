using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nameless.Microservices.Infrastructure.EntityFrameworkCore.Entities;

namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore;

public class AuditableSaveChangesInterceptor : SaveChangesInterceptor {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _timeProvider;

    public AuditableSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor, TimeProvider timeProvider) {
        _httpContextAccessor = httpContextAccessor;
        _timeProvider = timeProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
        var dbContext = eventData.Context;
        if (dbContext is null) {
            throw new InvalidOperationException("There is no database context available at the moment.");
        }

        var entries = dbContext.ChangeTracker.Entries<IAuditableEntity>();
        var now = _timeProvider.GetUtcNow();
        var currentUser = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        foreach (var entry in entries) {
            entry.Entity.MostRecentAuditee = currentUser;

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