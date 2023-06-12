using System.Collections;

namespace Nameless.ProducerConsumer {

    public sealed class Arguments : IEnumerable<KeyValuePair<string, object>> {

        #region Public Static Read-Only Properties

        public static Arguments Empty => new(new Dictionary<string, object>());

        #endregion

        #region Private Read-Only Fields

        private readonly IDictionary<string, object> _arguments;

        #endregion

        #region Public Constructors

        public Arguments(IDictionary<string, object> arguments) {
            Prevent.Null(arguments, nameof(arguments));

            _arguments = arguments;
        }

        #endregion

        #region Public Methods

        public object? Get(string key) {
            return _arguments.TryGetValue(key, out var result) ? result : default;
        }

        public object Get(string key, object fallback) {
            return _arguments.TryGetValue(key, out var result) ? result : fallback;
        }

        public T? Get<T>(string key) {
            var value = Get(key);

            return value is T result ? result : default;
        }

        public T Get<T>(string key, T fallback) {
            var value = Get(key);

            return value is T result ? result : fallback;
        }

        public bool Has(string key) => _arguments.ContainsKey(key);

        #endregion

        #region IEnumerable<KeyValuePair<string, object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _arguments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_arguments).GetEnumerator();

        #endregion
    }
}
