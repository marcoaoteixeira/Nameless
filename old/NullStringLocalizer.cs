using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Json {
    /// <summary>
    /// Null Object Pattern implementation for IStringLocalizer. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullStringLocalizer : IStringLocalizer {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullStringLocalizer.
        /// </summary>
        public static IStringLocalizer Instance => new NullStringLocalizer ();

        #endregion Public Static Properties

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullStringLocalizer () { }

        #endregion Static Constructors

        #region Private Constructors

        // Prevents the class from being constructed.
        private NullStringLocalizer () { }

        #endregion Private Constructors

        #region IStringLocalizer Members

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString (name, string.Format (name, arguments), resourceNotFound: true);

        public LocalizedString this[string name] => new LocalizedString (name, name, resourceNotFound: true);

        public IEnumerable<LocalizedString> GetAllStrings (bool includeParentCultures) => Enumerable.Empty<LocalizedString> ();

        public IStringLocalizer WithCulture (CultureInfo culture) => this;

        #endregion
    }
}