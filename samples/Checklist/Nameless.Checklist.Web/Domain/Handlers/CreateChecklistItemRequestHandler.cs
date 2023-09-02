using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers {
    public sealed class CreateChecklistItemRequestHandler : IRequestHandler<CreateChecklistItemRequest, ChecklistItemDto> {
        #region Private Read-Only Fields

        private readonly IRepository<ChecklistItem> _repository;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public CreateChecklistItemRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
            _repository = Guard.Against.Null(repository, nameof(repository));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        #endregion

        #region IRequestHandler<CreateTodoItemRequest, TodoItemDto> Members

        public async Task<ChecklistItemDto> Handle(CreateChecklistItemRequest request, CancellationToken cancellationToken) {
            var entity = _mapper.Map<ChecklistItem>(request);

            var result = await _repository.CreateAsync(entity, cancellationToken);

            var dto = _mapper.Map<ChecklistItemDto>(result);

            return dto;
        }

        #endregion
    }
}
