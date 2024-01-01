using System.Collections;

namespace Nameless.Infrastructure {
    public abstract class ArgCollection : IEnumerable<Arg> {
        #region Private Read-Only Fields

        private readonly Dictionary<string, object> _dictionary = [];

        #endregion

        #region Public Methods

        public void Set(Arg arg) {
            Guard.Against.Null(arg, nameof(arg));

            Set(arg.Name, arg.Value);
        }

        public void Set(string name, object value) {
            Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Guard.Against.Null(value, nameof(value));

            _dictionary[name] = value;
        }

        public object? Get(string name)
            => _dictionary.TryGetValue(name, out var value)
                ? value
                : null;

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
