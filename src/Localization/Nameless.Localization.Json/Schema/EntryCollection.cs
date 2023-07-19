using System.Collections;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Schema {
    public sealed class EntryCollection : IEnumerable<Entry> {
        #region Public Static Read-Only Properties

        public static EntryCollection Empty => new("NULL");

        #endregion

        #region Public Properties

        public string Source { get; }

        #endregion

        #region Private Properties

        public Dictionary<string, Entry> Entries { get; } = new();

        #endregion

        #region Public Constructors

        public EntryCollection(string source) {
            Source = Prevent.Against.NullOrWhiteSpace(source, nameof(source));
        }

        #endregion

        #region Public Methods

        public void Add(Entry entry) {
            Prevent.Against.Null(entry, nameof(entry));

            if (Entries.ContainsKey(entry.Key)) {
                Entries[entry.Key] = entry;
            } else {
                Entries.Add(entry.Key, entry);
            }
        }

        public bool TryGetValue(string key, out Entry? output)
            => Entries.TryGetValue(key, out output);

        public bool Equals(EntryCollection? other)
            => other != null
            && other.Source == Source;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as EntryCollection);

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
