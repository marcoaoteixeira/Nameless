using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers;

public sealed class GetChecklistItemRequestHandler : IRequestHandler<GetChecklistItemRequest, ChecklistItemDto?> {
    #region Private Read-Only Fields

    private readonly IRepository<ChecklistItem> _repository;
    private readonly IMapper _mapper;

    #endregion

    #region Public Constructors

    public GetChecklistItemRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
        _repository = Prevent.Argument.Null(repository);
        _mapper = Prevent.Argument.Null(mapper);
    }

    #endregion

    #region IRequestHandler<GetTodoItemRequest, ChecklistItemDto?> Members

    public async Task<ChecklistItemDto?> Handle(GetChecklistItemRequest request, CancellationToken cancellationToken) {
        var result = await _repository.GetAsync(request.Id, cancellationToken);

        var dto = _mapper.Map<ChecklistItemDto?>(result);

        return dto;
    }

    #endregion
}