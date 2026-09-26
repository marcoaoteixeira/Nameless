using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nameless.Microservices.Api.Data;
using Nameless.ObjectModel;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.Delete;

[Endpoint(
    route: "/{id:guid}",
    Verb = HttpVerbs.Delete,
    Description = "Deletes a chore from your list. Dodging responsibilities already?? 😒",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<Nothing>(StatusCode = StatusCodes.Status204NoContent)]
[ProducesProblemResponse]
[ProducesProblemResponse(StatusCode = StatusCodes.Status404NotFound)]
public partial class DeleteChoreEndpoint {
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DeleteChoreEndpoint> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="DeleteChoreEndpoint"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The application database context.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public DeleteChoreEndpoint(AppDbContext dbContext, ILogger<DeleteChoreEndpoint> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync([FromRoute] Guid id, CancellationToken cancellationToken) {
        var chore = await _dbContext.Chores.SingleOrDefaultAsync(
            item => item.ID == id, cancellationToken
        );

        if (chore is null) { return TypedResults.NotFound(); }

        _dbContext.Chores.Remove(chore);

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
