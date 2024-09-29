using MediatR;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers;

public sealed class DeleteChecklistItemRequestHandler : IRequestHandler<DeleteChecklistItemRequest, bool> {
    private readonly IRepository<ChecklistItem> _repository;

    public DeleteChecklistItemRequestHandler(IRepository<ChecklistItem> repository) {
        _repository = Prevent.Argument.Null(repository);
    }

    public Task<bool> Handle(DeleteChecklistItemRequest request, CancellationToken cancellationToken)
        => _repository.DeleteAsync(request.Id, cancellationToken);
}