using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Nameless.Orm.NHibernate {
    public sealed class Persister : IPersister {
        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion

        #region Public Constructors

        public Persister (ISession session) {
            Prevent.ParameterNull (session, nameof (session));

            _session = session;
        }

        #endregion

        #region Private Methods

        private Task ExecuteAsync<TEntity> (TEntity[] entities, Action<ISession, TEntity> action, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            progress = progress ?? NullProgress.Instance;

            using (var transaction = _session.BeginTransaction ()) {
                for (var idx = 0; idx < entities.Length; idx++) {
                    token.ThrowIfCancellationRequested ();

                    action (_session, entities[idx]);

                    // When inserting a collection of entities in the session object, as it keeps track
                    // of all those objects, it can become so full that it will throw an OutOfMemoryException.
                    // To avoid this scenario, we'll just flush/clear the session after every 20 entities interaction.
                    if (idx % 20 == 0) {
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
            return ExecuteAsync (entities, (session, entity) => { session.Delete (entity); }, progress, token);
        }

        public Task SaveAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            return ExecuteAsync (entities, (session, entity) => {
                session.SaveOrUpdate (entity);
            }, progress, token);
        }

        #endregion
    }
}