namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class DictionaryExtension {

        #region Public Static Methods

        /// <summary>
        /// Adds (or changes) a value onto a dictionary without fuss.
        /// </summary>
        /// <param name="self">The source dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue value) {
            Prevent.Null(self, nameof(self));

            if (!self.ContainsKey(key)) { self.Add(key, value); } else { self[key] = value; }
        }

        #endregion
    }
}