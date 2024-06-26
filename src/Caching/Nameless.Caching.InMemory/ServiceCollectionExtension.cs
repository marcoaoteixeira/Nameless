﻿using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.InMemory {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterCacheService(this IServiceCollection self)
            => self.AddSingleton<ICacheService, InMemoryCacheService>();

        #endregion
    }
}
