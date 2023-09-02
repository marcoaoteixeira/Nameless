using System.Linq.Expressions;
using System.Text.Json;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Repositories.Impl;

namespace Nameless.Checklist.Web.Domain.Repositories {
    public sealed class TodoItemRepository : IRepository<ChecklistItem> {
        #region Private Read-Only Fields

        private readonly IDictionary<Guid, ChecklistItem> _todoItems;

        #endregion

        #region Public Constructors

        public TodoItemRepository(string connectionString) {
            var json = File.ReadAllText(connectionString);
            var array = JsonSerializer.Deserialize<ChecklistItem[]>(json)
                ?? throw new ArgumentNullException("array");

            _todoItems = array.ToDictionary(
                _ => _.Id,
                _ => _
            );
        }

        #endregion

        #region IRepository<TodoItem> Members

        public Task<ChecklistItem> CreateAsync(ChecklistItem entity, CancellationToken cancellationToken = default) {
            if (!_todoItems.ContainsKey(entity.Id)) {
                _todoItems.Add(entity.Id, entity);
            }

            return Task.FromResult(entity);
        }
        
        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) {
            var result = _todoItems.Remove(id);

            return Task.FromResult(result);
        }
        
        public Task<ChecklistItem[]> ListAsync(Expression<Func<ChecklistItem, bool>>? where = null, Expression<Func<ChecklistItem, object>>? orderBy = null, bool orderByDescending = false, CancellationToken cancellationToken = default) {
            var query = _todoItems
                .Values
                .AsQueryable();

            if (where is not null) {
                query = query.Where(where);
            }

            if (orderBy is not null) {
                query = orderByDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            var result = query.ToArray();

            return Task.FromResult(result);
        }

        public Task<ChecklistItem?> GetAsync(Guid id, CancellationToken cancellationToken = default) {
            var result = _todoItems.ContainsKey(id)
                ? _todoItems[id]
                : null;

            return Task.FromResult(result);
        }

        public Task<bool> UpdateAsync(Guid id, ChecklistItem entity, CancellationToken cancellationToken = default) {
            if (_todoItems.ContainsKey(id)) {
                _todoItems[id] = entity;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        #endregion
    }
}
