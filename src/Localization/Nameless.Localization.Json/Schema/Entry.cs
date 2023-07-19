using System.Collections;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Schema {
    public sealed class Entry : IEnumerable<string> {
        #region Public Properties

        public string Key { get; }

        #endregion

        #region Private Properties

        private List<string> Values { get; } = new();

        #endregion

        #region Public Constructors

        public Entry(string key) {
            Key = Prevent.Against.NullOrWhiteSpace(key, nameof(key));
        }

        #endregion

        #region Public Methods

        public void Add(string value)
            => Values.Add(Prevent.Against.NullOrWhiteSpace(value, nameof(value)));

        public bool Equals(Entry? other)
            => other != null
            && other.Key == Key;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Entry);

        public override int GetHashCode()
            => SimpleHash.Compute(Key);

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
            => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Values.GetEnumerator();

        #endregion
    }
}
