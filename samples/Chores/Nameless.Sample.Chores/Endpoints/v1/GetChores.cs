using Nameless.Sample.Chores.Dtos;
using Nameless.Sample.Chores.Inputs;
using Nameless.Sample.Chores.Services;
using Nameless.Web.Endpoints;

namespace Nameless.Sample.Chores.Endpoints.v1;

public class GetChores : IEndpoint {
    private readonly IChoreService _choreService;

    public GetChores(IChoreService choreService) {
        _choreService = choreService;
    }

    public void Configure(IEndpointDescriptor descriptor) {
        descriptor
           .Get("/chores", HandleAsync)
           .AllowAnonymous()
           .WithName($"{nameof(GetChores)}_v1")
           .Produces<ChoreDto[]>()
           .ProducesProblem()
           .WithVersion(version: 1);
    }

    public async Task<ChoreDto[]> HandleAsync([AsParameters] GetChoresInput input) {
        var chores = await _choreService.ListAsync(input.Description, input.Done, input.Date);

        return [.. chores];
    }
}
