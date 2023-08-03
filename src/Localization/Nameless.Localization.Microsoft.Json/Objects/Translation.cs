using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nameless.Helpers;

namespace Nameless.Localization.Microsoft.Json.Objects {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Translation : IEnumerable<Region> {
        #region Public Static Read-Only Properties

        public static Translation Empty => new(new(string.Empty));

        #endregion

        #region Public Properties

        public CultureInfo Culture { get; }

        #endregion

        #region Internal Properties

        internal Dictionary<string, Region> Regions { get; } = new();

        #endregion

        #region Public Constructors

        public Translation(CultureInfo culture) {
            Culture = Prevent.Against.Null(culture, nameof(culture));
        }

        #endregion

        #region Public Methods

        public void Add(Region region) {
            Prevent.Against.Null(region, nameof(region));

            Regions.AddOrChange(region.Name, region);
        }

        public bool TryGetValue(string name, [NotNullWhen(true)] out Region? output)
            => Regions.TryGetValue(name, out output);

        public bool Equals(Translation? other)
            => other != null
            && other.Culture.Name == Culture.Name;

        #endregion

        #region Public Override Methods

        public override bool Equals(object? obj)
            => Equals(obj as Translation);

        public override int GetHashCode()
            => SimpleHash.Compute(Culture.Name);

        #endregion

        #region IEnumerable<EntryCollection> Members

        IEnumerator<Region> IEnumerable<Region>.GetEnumerator()
            => Regions.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Regions.Values.GetEnumerator();

        #endregion
    }
}
