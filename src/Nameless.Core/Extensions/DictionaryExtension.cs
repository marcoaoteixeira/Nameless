using System.Collections.Generic;

namespace Nameless {
    public static class DictionaryExtension {
        #region Public Static Methods

        /// <summary>
        /// Tries add or change a value to a dictionary without fuss.
        /// </summary>
        /// <param name="self">The source dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        public static void TryAddOrUpdate<TKey, TValue> (this IDictionary<TKey, TValue> self, TKey key, TValue value) {
            if (self == null) { return; }
            if (!self.ContainsKey (key)) { self.Add (key, value); } else { self[key] = value; }
        }

        #endregion
    }
}