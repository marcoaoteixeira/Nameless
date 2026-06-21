using Nameless.Microservices.Common.Chores.DTOs;
using Nameless.Microservices.Common.Chores.Inputs;
using Nameless.Microservices.Common.Chores.Outputs;
using Refit;

namespace Nameless.Microservices.Bff.Domains.Chores.External;

public interface IChoresHttpClient {
    [Post(path: "/api/v1/chores")]
    Task<CreateChoreOutput> CreateChoreAsync(CreateChoreInput input, CancellationToken cancellationToken);

    [Delete(path: "/api/v1/chores/{id}")]
    Task DeleteChoreAsync(Guid id, CancellationToken cancellationToken);

    [Get(path: "/api/v1/chores")]
    Task<ChoreDto[]> ListChoresAsync(ListChoresInput input, CancellationToken cancellationToken);

    [Post(path: "/api/v1/chores/{id}")]
    Task MarkDoneAsync(Guid id, CancellationToken cancellationToken);
}