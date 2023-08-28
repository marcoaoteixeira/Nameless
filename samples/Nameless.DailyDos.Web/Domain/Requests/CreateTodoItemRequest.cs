using MediatR;
using Nameless.DailyDos.Web.Domain.Dtos;

namespace Nameless.DailyDos.Web.Domain.Requests {
    public sealed class CreateTodoItemRequest : IRequest<TodoItemDto> {
        #region Public Properties

        public string Description { get; set; } = null!;

        #endregion
    }
}
