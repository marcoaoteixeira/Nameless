using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers {
    public sealed class ListChecklistItemsRequestHandler : IRequestHandler<ListChecklistItemsRequest, ChecklistItemDto[]> {
        #region Private Read-Only Fields

        private readonly IRepository<ChecklistItem> _repository;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public ListChecklistItemsRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
            _repository = Guard.Against.Null(repository, nameof(repository));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        #endregion

        #region IRequestHandler<GetTodoItemRequest, ChecklistItemDto[]> Members

        public async Task<ChecklistItemDto[]> Handle(ListChecklistItemsRequest request, CancellationToken cancellationToken) {
            Expression<Func<ChecklistItem, bool>> where = entity
                => entity.Description.Contains(request.DescriptionLike ?? string.Empty) &&
                   (entity.CheckedAt <= request.CheckedBefore.GetValueOrDefault(DateTime.UtcNow) || !request.CheckedBefore.HasValue);

            var result = await _repository.ListAsync(where: where, cancellationToken: cancellationToken);
            var dtos = _mapper.Map<ChecklistItemDto[]>(result);

            return dtos;
        }

        #endregion
    }
}
