using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Persistence.EntityFrameworkCore {
    public class Persister : IPersister {

        #region Public Constants

        public const int DEFAULT_BULK_SIZE = 32;

        #endregion

        #region Private Read-Only Fields

        private readonly DbContext _context;
        private readonly int _currentBulkSize;

        #endregion

        #region Public Constructors

        public Persister (DbContext context, int bulkSize = DEFAULT_BULK_SIZE) {
            Prevent.ParameterNull (context, nameof (context));

            _context = context;
            _currentBulkSize = bulkSize > 0 ? bulkSize : DEFAULT_BULK_SIZE;
        }

        #endregion

        #region Private Methods

        private Task ExecuteAsync<TEntity> (Action<DbSet<TEntity>, TEntity> action, TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            progress = progress ?? NullProgress.Instance;

            using (var transaction = _context.Database.BeginTransaction ()) {
                var set = _context.Set<TEntity> ();
                for (var idx = 0; idx < entities.Length; idx++) {
                    token.ThrowIfCancellationRequested ();

                    action (set, entities[idx]);

                    // When inserting a collection of entities in the context object, as it keeps track
                    // of all those objects, it can become so full that it will slow down.
                    // To avoid this scenario, we'll just save changes in the context after every configured bulk size.
                    if (idx % _currentBulkSize == 0) {
                        _context.SaveChanges ();
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

            return ExecuteAsync ((set, entity) => set.Remove (entity), entities, progress, token);
        }

        public Task SaveAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            return ExecuteAsync (async (set, entity) => {
                var exists = await set.ContainsAsync (entity, token);
                if (!exists) { _ = set.Add (entity); }
                else { _ = set.Update (entity); }
            }, entities, progress, token);
        }

        #endregion
    }
}