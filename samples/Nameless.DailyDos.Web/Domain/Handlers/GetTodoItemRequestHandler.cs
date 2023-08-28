using AutoMapper;
using MediatR;
using Nameless.DailyDos.Web.Domain.Dtos;
using Nameless.DailyDos.Web.Domain.Entities;
using Nameless.DailyDos.Web.Domain.Repositories.Impl;
using Nameless.DailyDos.Web.Domain.Requests;

namespace Nameless.DailyDos.Web.Domain.Handlers {
    public sealed class GetTodoItemRequestHandler : IRequestHandler<GetTodoItemRequest, TodoItemDto?> {
        #region Private Read-Only Fields

        private readonly IRepository<TodoItem> _repository;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public GetTodoItemRequestHandler(IRepository<TodoItem> repository, IMapper mapper) {
            _repository = Guard.Against.Null(repository, nameof(repository));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        #endregion

        #region IRequestHandler<GetTodoItemRequest, TodoItemDto?> Members

        public async Task<TodoItemDto?> Handle(GetTodoItemRequest request, CancellationToken cancellationToken) {
            var result = await _repository.GetAsync(request.Id, cancellationToken);

            var dto = _mapper.Map<TodoItemDto?>(result);

            return dto;
        }

        #endregion
    }
}
