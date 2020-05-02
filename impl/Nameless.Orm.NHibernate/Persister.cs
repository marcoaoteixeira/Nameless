using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class Persister : IPersister {
        #region Public Constants

        public const int DEFAULT_BULK_SIZE = 32;

        #endregion

        #region Private Read-Only Fields

        private readonly ISession _session;
        private readonly int _currentBulkSize;

        #endregion

        #region Public Constructors

        public Persister (ISession session, int bulkSize = DEFAULT_BULK_SIZE) {
            Prevent.ParameterNull (session, nameof (session));

            _session = session;
            _currentBulkSize = bulkSize > 0 ? bulkSize : DEFAULT_BULK_SIZE;
        }

        #endregion

        #region Private Methods

        private Task ExecuteAsync<TEntity> (TEntity[] entities, Action<ISession, TEntity> action, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            progress = progress ?? NullProgress.Instance;

            using (var transaction = _session.BeginTransaction ()) {
                for (var idx = 0; idx < entities.Length; idx++) {
                    token.ThrowIfCancellationRequested ();

                    action (_session, entities[idx]);

                    // When inserting a collection of entities in the session object, as it keeps track
                    // of all those objects, it can become so full that it will throw an OutOfMemoryException.
                    // To avoid this scenario, we'll just flush/clear the session after every configured bulk size.
                    if (idx % _currentBulkSize == 0) {
                        _session.Flush ();
                        _session.Clear ();
                    }

                    progress.Report (idx + 1);
                }
                transaction.Commit ();
            }
            return Task.CompletedTask;
        }

        #endregion

        #region IPersister Members

        public Task DeleteAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            return ExecuteAsync (entities, (session, entity) => session.Delete (entity), progress, token);
        }

        public Task SaveAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            return ExecuteAsync (entities, (session, entity) => session.SaveOrUpdate (entity), progress, token);
        }

        #endregion
    }
}