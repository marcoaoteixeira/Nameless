namespace Nameless.Caching {

    public static class CacheExtension {

        #region Public Static Methods

        public static Task<T?> GetAsync<T>(this ICache self, string key, CancellationToken cancellationToken = default) {
            Prevent.Null(self, nameof(self));

            return self
                .GetAsync(key, cancellationToken)
                .ContinueWith(antecedent =>
                    (antecedent.Result is T result) ? result : default
                , cancellationToken);
        }

        #endregion
    }
}