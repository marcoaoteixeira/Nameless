using System.Threading;
using System.Threading.Tasks;

namespace Nameless.Persistence {
    public static class PersisterExtension {
        #region Public Static Methods

        public static Task SaveAsync<TEntity> (this IPersister self, TEntity entity, CancellationToken token = default) where TEntity : class {
            if (self == null) { return Task.CompletedTask; }

            return self.SaveAsync (new [] { entity }, NullProgress.Instance, token);
        }

        public static Task DeleteAsync<TEntity> (this IPersister self, TEntity entity, CancellationToken token = default) where TEntity : class {
            if (self == null) { return Task.CompletedTask; }

            return self.DeleteAsync (new [] { entity }, NullProgress.Instance, token);
        }

        #endregion
    }
}