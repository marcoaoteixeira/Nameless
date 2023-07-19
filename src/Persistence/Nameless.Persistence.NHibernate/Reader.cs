using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

namespace Nameless.Persistence.NHibernate {
    public sealed class Reader : IReader {
        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion

        #region Public Constructors

        public Reader(ISession session) {
            Prevent.Against.Null(session, nameof(session));

            _session = session;
        }

        #endregion

        #region IReader Members

        public Task<IList<TEntity>> FindAsync<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>>? orderBy = null, bool orderDescending = false, CancellationToken cancellationToken = default) where TEntity : class {
            Prevent.Against.Null(filter, nameof(filter));

            var query = _session.Query<TEntity>();

            if (orderBy != null) {
                query = orderDescending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            var result = query
                .Where(filter)
                .ToListAsync(cancellationToken);

            return result;
        }

        public Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : class {
            Prevent.Against.Null(filter, nameof(filter));

            return _session.Query<TEntity>().AnyAsync(filter);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class {
            return _session.Query<TEntity>();
        }

        #endregion
    }
}
