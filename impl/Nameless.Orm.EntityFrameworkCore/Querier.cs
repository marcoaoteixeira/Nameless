using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Orm.EntityFrameworkCore {
    public sealed class Querier : IQuerier {
        #region Private Properties

        private DbContext DatabaseContext { get; }

        #endregion

        #region Public Constructors

        public Querier (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            DatabaseContext = context;
        }

        #endregion

        #region Private Methods

        private DbSet<TEntity> GetDbSet<TEntity> () where TEntity : class {
            return DatabaseContext.Set<TEntity> ();
        }

        #endregion

        #region IQuerier Members

        public Task<TEntity> FindOneAsync<TEntity> (object id, CancellationToken token = default) where TEntity : class {
            return GetDbSet<TEntity> ().FindAsync (
                keyValues: new[] { id },
                cancellationToken: token
            );
        }

        public Task<TEntity> FindOneAsync<TEntity> (Expression<Func<TEntity, bool>> expression, CancellationToken token = default) where TEntity : class {
            return GetDbSet<TEntity> ().SingleOrDefaultAsync (
                predicate: expression,
                cancellationToken: token
            );
        }

        public IAsyncEnumerable<TEntity> FindAllAsync<TEntity> (Expression<Func<TEntity, bool>> expression) where TEntity : class {
            return GetDbSet<TEntity> ().Where (expression).ToAsyncEnumerable ();
        }

        public IQueryable<TEntity> Query<TEntity> () where TEntity : class {
            return GetDbSet<TEntity> ();
        }

        #endregion Methods
    }
}