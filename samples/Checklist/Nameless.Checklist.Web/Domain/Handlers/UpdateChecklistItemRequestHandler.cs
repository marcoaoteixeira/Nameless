using AutoMapper;
using MediatR;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers;

public sealed class UpdateChecklistItemRequestHandler : IRequestHandler<UpdateChecklistItemRequest, ChecklistItemDto?> {
    private readonly IRepository<ChecklistItem> _repository;
    private readonly IMapper _mapper;

    public UpdateChecklistItemRequestHandler(IRepository<ChecklistItem> repository, IMapper mapper) {
        _repository = Prevent.Argument.Null(repository);
        _mapper = Prevent.Argument.Null(mapper);
    }

    public async Task<ChecklistItemDto?> Handle(UpdateChecklistItemRequest request, CancellationToken cancellationToken) {
        var current = await _repository.GetAsync(request.Id, cancellationToken);

        if (current is null) {
            return null;
        }

        var update = current with {
            Description = request.Description,
            CheckedAt = request.CheckedAt,
        };

        var result = await _repository.UpdateAsync(request.Id, update, cancellationToken);

        return result
            ? _mapper.Map<ChecklistItemDto>(update)
            : null;
    }
}