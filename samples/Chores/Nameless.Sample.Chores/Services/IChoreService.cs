using Nameless.Sample.Chores.Dtos;

namespace Nameless.Sample.Chores.Services;

public interface IChoreService {
    Task<ChoreDto> SaveAsync(ChoreDto chore, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<ChoreDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<ChoreDto>> ListAsync(string? description = null, bool? done = null, DateOnly? date = null);
}