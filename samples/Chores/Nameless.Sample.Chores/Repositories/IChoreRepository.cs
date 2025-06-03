using Nameless.Sample.Chores.Entities;

namespace Nameless.Sample.Chores.Repositories;

public interface IChoreRepository {
    Task SaveAsync(Chore chore, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Chore?> GetAsync(Guid id, CancellationToken cancellationToken);

    IQueryable<Chore> Query();
}