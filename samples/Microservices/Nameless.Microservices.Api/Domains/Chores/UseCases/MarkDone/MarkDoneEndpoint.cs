using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.Api.Data;
using Nameless.ObjectModel;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.MarkDone;

[Endpoint(
    route: "/{id:guid}",
    Verb = HttpVerbs.Post,
    Description = "Marks the chore as done. Go ahead, you've earned a cookie. 🍪",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<Nothing>(StatusCode = StatusCodes.Status204NoContent)]
[ProducesProblemResponse]
[ProducesProblemResponse(StatusCode = StatusCodes.Status404NotFound)]
public partial class MarkDoneEndpoint {
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<MarkDoneEndpoint> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MarkDoneEndpoint"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The application database context.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public MarkDoneEndpoint(AppDbContext dbContext, TimeProvider timeProvider, ILogger<MarkDoneEndpoint> logger) {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync([FromRoute] Guid id, CancellationToken cancellationToken) {
        var chore = await _dbContext.Chores.SingleOrDefaultAsync(
            item => item.ID == id,
            cancellationToken
        );

        if (chore is null) { return TypedResults.NotFound(); }

        if (chore.ConclusionDate is not null) { return TypedResults.NoContent(); }

        chore.ConclusionDate = _timeProvider.GetUtcNow();

        try {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) {
            _logger.Failure(ex);

            return TypedResults.InternalServerError(ex.Message);
        }

        return TypedResults.NoContent();
    }
}
