using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nameless.Helpers;

namespace Nameless.Localization.Json.Schema {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    public sealed class Translation : IEnumerable<Container> {
        #region Public Properties

        public CultureInfo Culture { get; }

        #endregion

        #region Private Properties

        private Dictionary<string, Container> Containers { get; } = new();

        #endregion

        #region Public Constructors

        public Translation(CultureInfo culture) {
            Culture = Prevent.Against.Null(culture, nameof(culture));
        }

        #endregion

        #region Public Methods

        public void Add(Container container) {
            Prevent.Against.Null(container, nameof(container));

            Containers.AddOrChange(container.Source, container);
        }

        public bool TryGetValue(string source, [NotNullWhen(true)] out Container? output)
            => Containers.TryGetValue(source, out output);

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

        public IEnumerator<Container> GetEnumerator()
            => Containers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Containers.Values.GetEnumerator();

        #endregion
    }
}
