namespace Nameless.ProducerConsumer.RabbitMQ {
    internal static class DictionaryExtension {
        #region Internal Static Methods

        internal static object? Get(this IDictionary<string, object> self, string key) {
            return self.TryGetValue(key, out var result) ? result : default;
        }

        internal static object Get(this IDictionary<string, object> self, string key, object fallback) {
            return self.TryGetValue(key, out var result) ? result : fallback;
        }


        internal static T? Get<T>(this IDictionary<string, object> self, string key) {
            var value = Get(self, key);

            return value is T result ? result : default;
        }

        internal static T Get<T>(this IDictionary<string, object> self, string key, T fallback) {
            var value = Get(self, key);

            return value is T result ? result : fallback;
        }

        #endregion
    }
}
