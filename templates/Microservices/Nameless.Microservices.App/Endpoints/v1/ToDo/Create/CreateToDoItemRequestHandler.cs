using Nameless.Mediator.Requests;
using Nameless.Microservices.App.Data;
using Nameless.Microservices.App.Entities;
using Nameless.Microservices.App.Internals;
using Nameless.ObjectModel;

namespace Nameless.Microservices.App.Endpoints.v1.ToDo.Create;

public sealed class CreateToDoItemRequestHandler : IRequestHandler<CreateToDoItemRequest, CreateToDoItemResponse> {
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CreateToDoItemRequestHandler> _logger;

    public CreateToDoItemRequestHandler(ApplicationDbContext dbContext, ILogger<CreateToDoItemRequestHandler> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CreateToDoItemResponse> HandleAsync(CreateToDoItemRequest request, CancellationToken cancellationToken) {
        var todo = new ToDoItem {
            ID = Guid.CreateVersion7(),
            Summary = request.Summary,
            DueDate = request.DueDate,
            ConclusionDate = null
        };

        try
        {
            var entity = await _dbContext.Set<ToDoItem>().AddAsync(todo, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity.Entity;
        }
        catch (Exception ex)
        {
            _logger.CreateToDoItemFailed(ex);

            return Error.Failure(ex.Message);
        }
    }
}