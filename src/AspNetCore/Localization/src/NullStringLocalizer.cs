using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nameless.AspNetCore.Localization {
    public sealed class NullStringLocalizer : Microsoft.Extensions.Localization.IStringLocalizer {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of NullLogger.
        /// </summary>
        public static Microsoft.Extensions.Localization.IStringLocalizer Instance { get; } = new NullStringLocalizer ();

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

        #region Microsoft.Extensions.Localization.IStringLocalizer

        public Microsoft.Extensions.Localization.LocalizedString this [string name] => new Microsoft.Extensions.Localization.LocalizedString (name, name);

        public Microsoft.Extensions.Localization.LocalizedString this [string name, params object[] arguments] => new Microsoft.Extensions.Localization.LocalizedString (name, string.Format (name, arguments));

        public IEnumerable<Microsoft.Extensions.Localization.LocalizedString> GetAllStrings (bool includeParentCultures) => Enumerable.Empty<Microsoft.Extensions.Localization.LocalizedString> ();

        public Microsoft.Extensions.Localization.IStringLocalizer WithCulture (CultureInfo culture) => this;

        #endregion
    }
}