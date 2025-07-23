using Nameless.Mediator.Requests;
using Nameless.Microservices.App.Data;
using Nameless.Microservices.App.Domains.Entities;

namespace Nameless.Microservices.App.Domains.UseCases;

public class CreateToDoRequestHandler : IRequestHandler<CreateToDoRequest, ToDo> {
    private readonly ApplicationDbContext _dbContext;

    public CreateToDoRequestHandler(ApplicationDbContext dbContext) {
        _dbContext = Prevent.Argument.Null(dbContext);
    }

    public async Task<ToDo> HandleAsync(CreateToDoRequest request, CancellationToken cancellationToken) {
        var todo = new ToDo {
            Id = Guid.NewGuid(),
            Summary = request.Summary,
            DueDate = request.DueDate
        };

        var entity = await _dbContext.Set<ToDo>()
                                     .AddAsync(todo, cancellationToken)
                                     .ConfigureAwait(continueOnCapturedContext: false);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return entity.Entity;
    }
}