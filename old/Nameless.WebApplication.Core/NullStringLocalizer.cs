using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Nameless.WebApplication {
    /// <summary>
    /// Null Object Pattern implementation for IStringLocalizer. (see: https://en.wikipedia.org/wiki/Null_Object_pattern)
    /// </summary>
    public sealed class NullStringLocalizer : IStringLocalizer {

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullStringLocalizer.
        /// </summary>
        public static IStringLocalizer Instance { get; } = new NullStringLocalizer ();

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

        /// <inheritdoc />
        public LocalizedString this[string name] {
            get { return new LocalizedString (name, name); }
        }

        /// <inheritdoc />
        public LocalizedString this[string name, params object[] arguments] {
            get { return new LocalizedString (name, string.Format (name, arguments)); }
        }

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings (bool includeParentCultures) => Array.Empty<LocalizedString> ();

        /// <inheritdoc />
        public IStringLocalizer WithCulture (CultureInfo culture) => this;

        #endregion IStringLocalizer Members
    }
}