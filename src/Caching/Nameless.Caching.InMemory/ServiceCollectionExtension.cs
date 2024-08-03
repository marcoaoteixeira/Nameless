using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.InMemory {
    /// <summary>
    /// Extension methods for In-Memory cache service.
    /// </summary>
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers the current implementation of <see cref="ICacheService"/>.
        /// </summary>
        /// <param name="self">The current <see cref="IServiceCollection"/> instance.</param>
        /// <returns>The current <see cref="IServiceCollection"/> so other actions can be chained.</returns>
        public static IServiceCollection RegisterCacheService(this IServiceCollection self)
            => self.AddSingleton<ICacheService, InMemoryCacheService>();

        #endregion
    }
}
