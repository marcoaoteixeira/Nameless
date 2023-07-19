using System.Collections;

namespace Nameless.ProducerConsumer {
    public abstract class ArgCollection : IEnumerable<Arg> {
        #region Private Read-Only Fields

        private readonly Dictionary<string, object> _dictionary = new();

        #endregion

        #region Public Methods

        public void Set(Arg arg) {
            Prevent.Against.Null(arg, nameof(arg));

            Set(arg.Name, arg.Value);
        }

        public void Set(string name, object value) {
            Prevent.Against.NullOrWhiteSpace(name, nameof(name));
            Prevent.Against.Null(value, nameof(value));

            if (!_dictionary.ContainsKey(name)) {
                _dictionary.Add(name, value);
            } else {
                _dictionary[name] = value;
            }
        }

        public object? Get(string name)
            => _dictionary.TryGetValue(name, out var value) ? value : null;

        #endregion

        #region Private Methods

        private IEnumerable<Arg> GetArgs() {
            foreach (var item in _dictionary) {
                yield return new(item.Key, item.Value);
            }
        }

        #endregion

        #region IEnumerable<Arg> Members

        public IEnumerator<Arg> GetEnumerator()
            => GetArgs().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetArgs().GetEnumerator();

        #endregion
    }
}
