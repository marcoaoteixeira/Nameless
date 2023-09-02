using System.Linq.Expressions;

namespace Nameless.Checklist.Web.Domain.Repositories.Impl {
    public interface IRepository<TEntity> where TEntity : class {
        #region Methods

        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TEntity[]> ListAsync(Expression<Func<TEntity, bool>>? where = null, Expression<Func<TEntity, object>>? orderBy = null, bool orderByDescending = false, CancellationToken cancellationToken = default);

        #endregion
    }
}
