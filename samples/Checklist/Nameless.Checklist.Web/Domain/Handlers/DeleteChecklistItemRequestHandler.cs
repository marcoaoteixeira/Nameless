using MediatR;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Handlers {
    public sealed class DeleteChecklistItemRequestHandler : IRequestHandler<DeleteChecklistItemRequest, bool> {
        #region Private Read-Only Fields

        private readonly IRepository<ChecklistItem> _repository;

        #endregion

        #region Public Constructors

        public DeleteChecklistItemRequestHandler(IRepository<ChecklistItem> repository) {
            _repository = Guard.Against.Null(repository, nameof(repository));
        }

        #endregion

        #region IRequestHandler<DeleteChecklistItemRequest, bool> Members

        public Task<bool> Handle(DeleteChecklistItemRequest request, CancellationToken cancellationToken)
            => _repository.DeleteAsync(request.Id, cancellationToken);

        #endregion
    }
}
