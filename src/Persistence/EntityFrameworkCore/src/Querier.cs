using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Persistence.EntityFrameworkCore {
    public sealed class Querier : IQuerier {
        #region Private Read-Only Fields

        private readonly DbContext _context;

        #endregion

        #region Public Constructors

        public Querier (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            _context = context;
        }

        #endregion

        #region IQuerier Members

        public Task<TEntity> FindOneAsync<TEntity> (object id, CancellationToken token = default) where TEntity : class {
            return _context.Set<TEntity> ().FindAsync (
                keyValues: new [] { id },
                cancellationToken: token
            ).AsTask ();
        }

        public Task<TEntity> FindOneAsync<TEntity> (Expression<Func<TEntity, bool>> expression, CancellationToken token = default) where TEntity : class {
            return _context.Set<TEntity> ().SingleOrDefaultAsync (
                predicate: expression,
                cancellationToken: token
            );
        }

        public IAsyncEnumerable<TEntity> FindAllAsync<TEntity> (Expression<Func<TEntity, bool>> expression) where TEntity : class {
            return _context.Set<TEntity> ().Where (expression).ToAsyncEnumerable ();
        }

        public IQueryable<TEntity> Query<TEntity> () where TEntity : class {
            return _context.Set<TEntity> ();
        }

        #endregion Methods
    }
}