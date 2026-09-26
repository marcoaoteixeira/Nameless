using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.Api.Data;
using Nameless.Microservices.Api.Domains.Chores.Entities;
using Nameless.Microservices.Common.Chores.DTOs;
using Nameless.Microservices.Common.Chores.Inputs;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.List;

[Endpoint(
    Description = "Lists all your daily chores. Let's see what today's version of you promised yesterday's version. 🥲",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<ChoreDto[]>]
public partial class ListChoresEndpoint {
    private readonly AppDbContext _dbContext;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="ListChoresEndpoint"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The application database context.
    /// </param>
    public ListChoresEndpoint(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    public async Task<IResult> HandleAsync([AsParameters] ListChoresInput input, CancellationToken cancellationToken) {
        var chores = await _dbContext.Chores.AsNoTracking()
            .WithinTitle(input.Title)
            .WithinDescription(input.Description)
            .BetweenDueDate(input.DueDateStart, input.DueDateEnd)
            .BetweenConclusionDate(input.ConclusionDateStart, input.ConclusionDateEnd)
            .IsDone(input.Done)
            .ToArrayAsync<Chore>(cancellationToken);

        return TypedResults.Ok(chores);
    }
}