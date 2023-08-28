using MediatR.Pipeline;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.DailyDos.Web.Domain.Dtos;
using Nameless.DailyDos.Web.Domain.Requests;

namespace Nameless.DailyDos.Web.Domain.PostProcessors {
    public sealed class CreateTodoItemRequestPostProcessor : IRequestPostProcessor<CreateTodoItemRequest, TodoItemDto> {
        #region Private Read-Only Fields

        private readonly ILogger<CreateTodoItemRequestPostProcessor> _logger;

        #endregion

        #region Public Constructors

        public CreateTodoItemRequestPostProcessor(ILogger<CreateTodoItemRequestPostProcessor> logger) {
            _logger = logger ?? NullLogger<CreateTodoItemRequestPostProcessor>.Instance;
        }

        #endregion

        #region IRequestPostProcessor<CreateTodoItemRequest, TodoItemDto> Members

        public Task Process(CreateTodoItemRequest request, TodoItemDto response, CancellationToken cancellationToken) {
            _logger.LogInformation("Post Processor Activated");

            return Task.CompletedTask;
        }

        #endregion

    }
}
