using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Objects.Translation {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Trunk : IEnumerable<Branch> {
        #region Public Properties

        public CultureInfo Culture { get; }

        #endregion

        #region Internal Properties

        internal Dictionary<string, Branch> Branches { get; } = new();

        #endregion

        #region Public Constructors

        public Trunk(CultureInfo culture) {
            Culture = Prevent.Against.Null(culture, nameof(culture));
        }

        #endregion

        #region Public Methods

        public void Add(Branch branch) {
            Prevent.Against.Null(branch, nameof(branch));

            Branches.AddOrChange(branch.Name, branch);
        }

        public bool TryGetValue(string name, [NotNullWhen(true)] out Branch? output)
            => Branches.TryGetValue(name, out output);

        public bool Equals(Trunk? other)
            => other != null
            && other.Culture.Name == Culture.Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Trunk);

        public override int GetHashCode()
            => SimpleHash.Compute(Culture.Name);

        #endregion

        #region IEnumerable<EntryCollection> Members

        IEnumerator<Branch> IEnumerable<Branch>.GetEnumerator()
            => Branches.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Branches.Values.GetEnumerator();

        #endregion
    }
}
