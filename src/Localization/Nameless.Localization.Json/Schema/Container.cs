using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Schema {
    public sealed class Container : IEnumerable<Entry> {
        #region Public Static Read-Only Properties

        public static Container Empty => new(string.Empty);

        #endregion

        #region Public Properties

        public string Source { get; }

        #endregion

        #region Private Properties

        private Dictionary<string, Entry> Entries { get; } = new();

        #endregion

        #region Public Constructors

        public Container(string source) {
            Source = Prevent.Against.Null(source, nameof(source));
        }

        #endregion

        #region Public Methods

        public void Add(Entry entry) {
            Prevent.Against.Null(entry, nameof(entry));

            Entries.AddOrChange(entry.Key, entry);
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out Entry? output)
            => Entries.TryGetValue(key, out output);

        public bool Equals(Container? other)
            => other != null
            && other.Source == Source;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Container);

        public override int GetHashCode()
            => SimpleHash.Compute(Source);

        #endregion

        #region IEnumerable<Entry> Members

        public IEnumerator<Entry> GetEnumerator()
            => Entries.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Entries.Values.GetEnumerator();

        #endregion
    }
}
