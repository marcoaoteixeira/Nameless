using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers;

public sealed class GetChecklistItemRequestHandler : IRequestHandler<GetChecklistItemRequest, ChecklistItemDto?> {
    private readonly IRepository<ChecklistItem> _repository;
    private readonly IMapper _mapper;

    public GetChecklistItemRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
        _repository = Prevent.Argument.Null(repository);
        _mapper = Prevent.Argument.Null(mapper);
    }

    public async Task<ChecklistItemDto?> Handle(GetChecklistItemRequest request, CancellationToken cancellationToken) {
        var result = await _repository.GetAsync(request.Id, cancellationToken);

        var dto = _mapper.Map<ChecklistItemDto?>(result);

        return dto;
    }
}