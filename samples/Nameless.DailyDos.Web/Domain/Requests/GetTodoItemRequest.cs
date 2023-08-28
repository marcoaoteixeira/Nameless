using MediatR;
using Nameless.DailyDos.Web.Domain.Dtos;

namespace Nameless.DailyDos.Web.Domain.Requests {
    public sealed class GetTodoItemRequest : IRequest<TodoItemDto?> {
        #region Public Properties

        public Guid Id { get; set; }

        #endregion
    }
}
