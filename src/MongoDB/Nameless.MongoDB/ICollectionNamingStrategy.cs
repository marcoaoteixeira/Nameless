﻿namespace Nameless.MongoDB {
    public interface ICollectionNamingStrategy {
        #region Methods

        string? GetCollectionName(Type? type);

        #endregion
    }

    public static class CollectionNamingStrategyExtension {
        #region Public Static Methods

        public static string? GetCollectionName<T>(this ICollectionNamingStrategy self)
            => self.GetCollectionName(typeof(T));

        #endregion
    }
}
