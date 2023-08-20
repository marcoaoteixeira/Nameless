using System.Text.Json;
using Nameless.Microservices.Dtos;
using Nameless.Microservices.Entities;

namespace Nameless.Microservices.Repositories {
    public sealed class TodoRepository {
        private readonly IDictionary<Guid, TodoItem> _todoItems;

        public TodoRepository(string connectionString) {
            var json = File.ReadAllText(connectionString);
            var array = JsonSerializer.Deserialize<TodoItem[]>(json)
                ?? throw new ArgumentNullException("array");

            _todoItems = array.ToDictionary(
                _ => _.Id,
                _ => _
            );
        }

        public TodoItemDto Create(string description) {
            var entity = new TodoItem(Guid.NewGuid(), description, DateTime.UtcNow);

            _todoItems.Add(entity.Id, entity);

            return new(entity.Id, entity.Description, entity.CreatedAt);
        }

        public TodoItemDto? GetByID(Guid id) {
            if (_todoItems.ContainsKey(id)) {
                var entity = _todoItems[id];

                return new(entity.Id, entity.Description, entity.CreatedAt, entity.FinishedAt);
            }

            return null;
        }

        public IEnumerable<TodoItemDto> List() {
            foreach (var entity in _todoItems.Values) {
                yield return new(entity.Id, entity.Description, entity.CreatedAt, entity.FinishedAt);
            }
        }

        public void Update(TodoItemDto todo) {
            if (!_todoItems.ContainsKey(todo.Id)) {
                return;
            }
            
            var current = _todoItems[todo.Id];
            var entity = new TodoItem(current.Id, todo.Description, current.CreatedAt, todo.FinishedAt);

            _todoItems[todo.Id] = entity;
        }

        public void Delete(Guid id) {
            if ( _todoItems.ContainsKey(id)) {
                _todoItems.Remove(id);
            }
        }
    }
}
