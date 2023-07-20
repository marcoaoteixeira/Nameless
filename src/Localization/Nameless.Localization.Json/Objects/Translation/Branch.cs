using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Objects.Translation {
    public sealed class Branch : IEnumerable<Leaf> {
        #region Public Static Read-Only Properties

        public static Branch Empty => new(string.Empty);

        #endregion

        #region Public Properties

        public string Name { get; }

        #endregion

        #region Internal Properties

        internal Dictionary<string, Leaf> Leaves { get; } = new();

        #endregion

        #region Public Constructors

        public Branch(string name) {
            Name = Prevent.Against.Null(name, nameof(name));
        }

        #endregion

        #region Public Methods

        public void Add(Leaf leaf) {
            Prevent.Against.Null(leaf, nameof(leaf));

            Leaves.AddOrChange(leaf.ID, leaf);
        }

        public bool TryGetValue(string id, [NotNullWhen(true)] out Leaf? output)
            => Leaves.TryGetValue(id, out output);

        public bool Equals(Branch? other)
            => other != null
            && other.Name == Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Branch);

        public override int GetHashCode()
            => SimpleHash.Compute(Name);

        #endregion

        #region IEnumerable<Message> Members

        IEnumerator<Leaf> IEnumerable<Leaf>.GetEnumerator()
            => Leaves.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Leaves.Values.GetEnumerator();

        #endregion
    }
}
