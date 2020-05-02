using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Orm {

    public sealed class Repository : IRepository {

        #region Private Read-Only Fields

        private readonly IDirectiveExecutor _directiveExecutor;
        private readonly IPersister _persister;
        private readonly IQuerier _querier;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Repository" />
        /// </summary>
        /// <param name="directiveExecutor">The directive executor.</param>
        /// <param name="persister">The persister.</param>
        /// <param name="querier">The querier.</param>
        public Repository (IDirectiveExecutor directiveExecutor, IPersister persister, IQuerier querier) {
            Prevent.ParameterNull (directiveExecutor, nameof (directiveExecutor));
            Prevent.ParameterNull (persister, nameof (persister));
            Prevent.ParameterNull (querier, nameof (querier));

            _directiveExecutor = directiveExecutor;
            _persister = persister;
            _querier = querier;
        }

        #endregion Public Constructors

        #region IRepository Members

        /// <inheritdoc />
        public Task SaveAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            return _persister.SaveAsync (entities, progress, token);
        }

        /// <inheritdoc />
        public Task<TResult> ExecuteDirectiveAsync<TResult, TDirective> (NameValueParameterSet parameters, IProgress<int> progress = null, CancellationToken token = default) where TDirective : IDirective<TResult> {
            return _directiveExecutor.ExecuteDirectiveAsync<TResult, TDirective> (parameters, progress, token);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<TEntity> FindAllAsync<TEntity> (Expression<Func<TEntity, bool>> expression) where TEntity : class {
            return _querier.FindAllAsync (expression);
        }

        /// <inheritdoc />
        public Task<TEntity> FindOneAsync<TEntity> (object id, CancellationToken token = default) where TEntity : class {
            return _querier.FindOneAsync<TEntity> (id, token);
        }

        /// <inheritdoc />
        public Task<TEntity> FindOneAsync<TEntity> (Expression<Func<TEntity, bool>> expression, CancellationToken token = default) where TEntity : class {
            return _querier.FindOneAsync (expression, token);
        }

        /// <inheritdoc />
        public IQueryable<TEntity> Query<TEntity> () where TEntity : class {
            return _querier.Query<TEntity> ();
        }

        /// <inheritdoc />
        public Task DeleteAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            return _persister.DeleteAsync (entities, progress, token);
        }

        #endregion IRepository Members
    }
}