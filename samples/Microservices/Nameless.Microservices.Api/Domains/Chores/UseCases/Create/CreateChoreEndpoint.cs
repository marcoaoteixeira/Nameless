using Microsoft.AspNetCore.Mvc;
using Nameless.Microservices.Api.Data;
using Nameless.Microservices.Api.Domains.Chores.Entities;
using Nameless.Microservices.Common.Chores.Inputs;
using Nameless.Microservices.Common.Chores.Outputs;
using Nameless.Web.Http.Endpoints;
using Nameless.Web.Http.Endpoints.Attributes.Produces;
using Nameless.Web.Http.Endpoints.Attributes.Validation;

namespace Nameless.Microservices.Api.Domains.Chores.UseCases.Create;

[Endpoint(
    Verb = HttpVerbs.Post,
    Name = "Create Chore",
    Description = "Creates a new chore in your daily list. Time to make Future You proud. 😁",
    Group = typeof(ChoresEndpointGroup)
)]
[ProducesResponse<CreateChoreOutput>]
[ProducesProblemResponse]
[ProducesValidationProblemResponse]
[EnableValidation]
public partial class CreateChoreEndpoint {
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateChoreEndpoint> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CreateChoreEndpoint"/> class.
    /// </summary>
    /// <param name="dbContext">
    ///     The application database context.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public CreateChoreEndpoint(AppDbContext dbContext, ILogger<CreateChoreEndpoint> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync([FromBody] CreateChoreInput input, CancellationToken cancellationToken) {
        var entity = new Chore {
            Title = input.Title,
            Description = input.Description,
            DueDate = input.DueDate?.ToUniversalTime()
        };

        var entry = await _dbContext.AddAsync(entity, cancellationToken);

        try {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) {
            _logger.Failure(ex);

            return TypedResults.InternalServerError(ex.Message);
        }

        return TypedResults.Ok(new CreateChoreOutput {
            ID = entry.Entity.ID
        });
    }
}
