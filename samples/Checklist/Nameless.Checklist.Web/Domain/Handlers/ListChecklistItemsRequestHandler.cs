using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers;

public sealed class ListChecklistItemsRequestHandler : IRequestHandler<ListChecklistItemsRequest, ChecklistItemDto[]> {
    private readonly IRepository<ChecklistItem> _repository;
    private readonly IMapper _mapper;

    public ListChecklistItemsRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
        _repository = Prevent.Argument.Null(repository);
        _mapper = Prevent.Argument.Null(mapper);
    }

    public async Task<ChecklistItemDto[]> Handle(ListChecklistItemsRequest request, CancellationToken cancellationToken) {
        Expression<Func<ChecklistItem, bool>> where = entity
            => entity.Description.Contains(request.DescriptionLike ?? string.Empty) &&
               (entity.CheckedAt <= (request.CheckedBefore ?? DateTime.UtcNow)|| !request.CheckedBefore.HasValue);

        var result = await _repository.ListAsync(where: where, cancellationToken: cancellationToken);
        var dtos = _mapper.Map<ChecklistItemDto[]>(result);

        return dtos;
    }
}