using Bogus;
using Nameless.Sample.Chores.Entities;

namespace Nameless.Sample.Chores.Repositories;

public class ChoreRepository : IChoreRepository {
    private static readonly List<Chore> Cache;

    static ChoreRepository() {
        Cache = new Faker<Chore>()
               .StrictMode(true)
               .RuleFor(item => item.ID, value => value.System.Random.Guid())
               .RuleFor(item => item.Description, value => value.Lorem.Sentence(10))
               .RuleFor(item => item.Done, value => value.Random.Bool())
               .RuleFor(item => item.Date, value => value.Date.BetweenDateOnly(new DateOnly(2000, 1, 1), new DateOnly(2099, 12, 31)))
               .Generate(10);
    }

    public Task SaveAsync(Chore chore, CancellationToken cancellationToken) {
        if (Cache.Contains(chore)) {
            Cache.Remove(chore);
        }

        Cache.Add(chore);

        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) {
        var chore = Cache.SingleOrDefault(chore => chore.ID == id);
        var result = false;

        if (chore is not null) {
            result = Cache.Remove(chore);
        }

        return Task.FromResult(result);
    }

    public Task<Chore?> GetAsync(Guid id, CancellationToken cancellationToken) {
        var chore = Cache.SingleOrDefault(chore => chore.ID == id);

        return Task.FromResult(chore);
    }

    public IQueryable<Chore> Query() {
        return Cache.AsQueryable();
    }
}