using Nameless.Sample.Chores.Dtos;
using Nameless.Sample.Chores.Entities;
using Nameless.Sample.Chores.Repositories;

namespace Nameless.Sample.Chores.Services;

public class ChoreService : IChoreService {
    private readonly IChoreRepository _choreRepository;

    public ChoreService(IChoreRepository choreRepository) {
        _choreRepository = choreRepository;
    }

    public async Task<ChoreDto> SaveAsync(ChoreDto chore, CancellationToken cancellationToken) {
        var entity = new Chore {
            ID = Guid.NewGuid(),
            Description = chore.Description,
            Done = chore.Done,
            Date = chore.Date
        };

        await _choreRepository.SaveAsync(entity, cancellationToken);

        return entity.ToDto();
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) {
        return _choreRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ChoreDto?> GetAsync(Guid id, CancellationToken cancellationToken) {
        var entity = await _choreRepository.GetAsync(id, cancellationToken);

        return entity?.ToDto();
    }

    public Task<IEnumerable<ChoreDto>> ListAsync(string? description = null, bool? done = null, DateOnly? date = null) {
        var query = _choreRepository.Query();

        if (description is not null) {
            query = query.Where(chore => chore.Description.Contains(description, StringComparison.OrdinalIgnoreCase));
        }

        if (done is not null) {
            query = query.Where(chore => chore.Done == done);
        }

        if (date is not null) {
            query = query.Where(chore => chore.Date == date);
        }

        var result = query.Select(chore => chore.ToDto()).AsEnumerable();

        return Task.FromResult(result);
    }
}