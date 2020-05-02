using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nameless.Orm.EntityFrameworkCore {
    public class Persister : IPersister {
        #region Private Properties

        private DbContext Context { get; }

        #endregion

        #region Public Constructors

        public Persister (DbContext context) {
            Prevent.ParameterNull (context, nameof (context));

            Context = context;
        }

        #endregion

        #region Private Methods

        private DbSet<TEntity> GetDbSet<TEntity> () where TEntity : class {
            return Context.Set<TEntity> ();
        }

        #endregion

        #region IPersister Members

        public Task DeleteAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            progress = progress ?? NullProgress.Instance;
            var counter = 0;
            using (var transaction = Context.Database.BeginTransaction ()) {
                foreach (var entity in entities) {
                    GetDbSet<TEntity> ().Remove (entity);
                    token.ThrowIfCancellationRequested ();
                    progress.Report (++counter);
                }
                transaction.Commit ();
            }
            return Task.CompletedTask;
        }

        public Task SaveAsync<TEntity> (TEntity[] entities, IProgress<int> progress = null, CancellationToken token = default) where TEntity : class {
            Prevent.ParameterNull (entities, nameof (entities));

            progress = progress ?? NullProgress.Instance;
            var counter = 0;
            using (var transaction = Context.Database.BeginTransaction ()) {
                foreach (var entity in entities) {
                    GetDbSet<TEntity> ().Add (entity);
                    token.ThrowIfCancellationRequested ();
                    progress.Report (++counter);
                }
                transaction.Commit ();
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}