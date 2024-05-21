using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Localization.Microsoft.Json.Objects {
    /// <summary>
    /// This class represents a translation, in other words, a file for a culture.
    /// </summary>
    [DebuggerDisplay("{Culture}")]
    public sealed record Translation(string Culture, Region[] Regions) {
        #region Public Static Read-Only Properties

        public static Translation Empty => new(string.Empty, []);

        #endregion

        #region Public Properties

        public string Culture { get; } = Culture;
        public Region[] Regions { get; } = Regions;

        #endregion

        #region Public Methods

        public bool TryGetRegion(string name, [NotNullWhen(true)] out Region? output) {
            var current = Regions.SingleOrDefault(item => name == item.Name);

            output = current;

            return current is not null;
        }

        #endregion
    }
}
