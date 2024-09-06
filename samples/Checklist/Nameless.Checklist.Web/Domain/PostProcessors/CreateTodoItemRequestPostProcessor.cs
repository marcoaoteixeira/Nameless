using MediatR.Pipeline;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.PostProcessors;

public sealed class CreateTodoItemRequestPostProcessor : IRequestPostProcessor<CreateChecklistItemRequest, ChecklistItemDto> {
    #region Private Read-Only Fields

    private readonly ILogger<CreateTodoItemRequestPostProcessor> _logger;

    #endregion

    #region Public Constructors

    public CreateTodoItemRequestPostProcessor(ILogger<CreateTodoItemRequestPostProcessor> logger) {
        _logger = logger ?? NullLogger<CreateTodoItemRequestPostProcessor>.Instance;
    }

    #endregion

    #region IRequestPostProcessor<CreateTodoItemRequest, TodoItemDto> Members

    public Task Process(CreateChecklistItemRequest request, ChecklistItemDto response, CancellationToken cancellationToken) {
        _logger.LogInformation("Post Processor Activated");

        return Task.CompletedTask;
    }

    #endregion

}