using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class Querier : IQuerier {
        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion

        #region Public Constructors

        public Querier (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            _session = session;
        }

        #endregion

        #region IQuerier Members
        public IAsyncEnumerable<TEntity> FindAllAsync<TEntity> (Expression<Func<TEntity, bool>> expression) where TEntity : class {
            Prevent.ParameterNull (expression, nameof (expression));
            return _session.Query<TEntity> ().Where (expression).ToAsyncEnumerable ();
        }

        public Task<TEntity> FindOneAsync<TEntity> (object id, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (id, nameof (id));
            return _session.GetAsync<TEntity> (id, token);
        }

        public Task<TEntity> FindOneAsync<TEntity> (Expression<Func<TEntity, bool>> expression, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (expression, nameof (expression));
            var entity = _session.Query<TEntity> ().SingleOrDefault (expression);
            return Task.FromResult (entity);
        }

        public IQueryable<TEntity> Query<TEntity> () where TEntity : class {
            return _session.Query<TEntity> ();
        }

        #endregion
    }
}