using Microsoft.AspNetCore.Mvc;
using Nameless.Sample.Chores.Dtos;
using Nameless.Sample.Chores.Inputs;
using Nameless.Sample.Chores.Services;
using Nameless.Web.Endpoints;
using Nameless.Web.Filters;

namespace Nameless.Sample.Chores.Endpoints.v1;

public class PostChore : IEndpoint {
    private readonly IChoreService _choreService;

    public PostChore(IChoreService choreService) {
        _choreService = choreService;
    }

    public void Configure(IEndpointDescriptor descriptor) {
        descriptor
           .Post("/chores", HandleAsync)
           .AllowAnonymous()
           .WithName($"{nameof(PostChore)}_v1")
           .WithFilter<ValidateEndpointFilter>()
           .Produces<ChoreDto>()
           .ProducesProblem()
           .ProducesValidationProblem()
           .WithVersion(version: 1);
    }

    public async Task<ChoreDto> HandleAsync([FromBody] PostChoreInput input, CancellationToken cancellationToken) {
        var chore = new ChoreDto {
            Description = input.Description,
            Done = input.Done,
            Date = input.Date
        };

        return await _choreService.SaveAsync(chore, cancellationToken);
    }
}
