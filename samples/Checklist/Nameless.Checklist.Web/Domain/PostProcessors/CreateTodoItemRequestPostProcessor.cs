using MediatR.Pipeline;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.PostProcessors;

public sealed class CreateTodoItemRequestPostProcessor : IRequestPostProcessor<CreateChecklistItemRequest, ChecklistItemDto> {
    private readonly ILogger<CreateTodoItemRequestPostProcessor> _logger;

    public CreateTodoItemRequestPostProcessor(ILogger<CreateTodoItemRequestPostProcessor> logger) {
        _logger = Prevent.Argument.Null(logger);
    }

    public Task Process(CreateChecklistItemRequest request, ChecklistItemDto response, CancellationToken cancellationToken) {
        _logger.LogInformation("Post Processor Activated");

        return Task.CompletedTask;
    }
}