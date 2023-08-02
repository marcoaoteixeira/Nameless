using Nameless.Microservice.Web.Dtos;
using Nameless.Microservice.Web.Repositories;

namespace Nameless.Microservice.Web.Services {
    public sealed class TodoService {
        private readonly TodoRepository _repository;

        public TodoService(TodoRepository repository) {
            _repository = repository;
        }

        public TodoItemDto Create(string description) {
            if (string.IsNullOrWhiteSpace(description)) {
                throw new ArgumentException("Parameter cannot be null, empty or white space.", nameof(description));
            }

            return _repository.Create(description);
        }

        public TodoItemDto? GetByID(Guid id)
            => _repository.GetByID(id);

        public IEnumerable<TodoItemDto> List(string? descriptionLike = null, DateTime? finishedBefore = null) {
            var query = _repository.List().AsQueryable();

            if (!string.IsNullOrWhiteSpace(descriptionLike)) {
                query = query.Where(_ => _.Description.Contains(descriptionLike));
            }

            if (finishedBefore.HasValue) {
                query = query.Where(_ => _.FinishedAt <= finishedBefore.Value);
            }

            return query;
        }

        public void SetDescription(Guid id, string description) {
            if (id == Guid.Empty) {
                throw new ArgumentException("Must provide entity ID.");
            }

            if (string.IsNullOrWhiteSpace(description)) {
                throw new ArgumentException("Description should not be null, empty or white space.");
            }

            var entity = _repository.GetByID(id)
                ?? throw new ArgumentNullException("Entity not found.");

            var dto = new TodoItemDto(id, description, entity.CreatedAt, entity.FinishedAt);

            _repository.Update(dto);
        }

        public void SetFinished(Guid id) {
            if (id == Guid.Empty) {
                throw new ArgumentException("Must provide entity ID.");
            }

            var entity = _repository.GetByID(id)
                ?? throw new ArgumentNullException("Entity not found.");

            var dto = new TodoItemDto(id, entity.Description, entity.CreatedAt, DateTime.UtcNow);

            _repository.Update(dto);
        }

        public void Delete(Guid id)
            => _repository.Delete(id);
    }
}
