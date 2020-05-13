using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nameless.Localization {
    public sealed class EmptyStringLocalizer : IStringLocalizer {
        #region Private Constructors

        private EmptyStringLocalizer (string baseName, string location, string cultureName) {
            Prevent.ParameterNullOrWhiteSpace (baseName, nameof (baseName));
            Prevent.ParameterNullOrWhiteSpace (location, nameof (location));
            Prevent.ParameterNullOrWhiteSpace (cultureName, nameof (cultureName));

            BaseName = baseName;
            Location = location;
            CultureName = cultureName;
        }

        #endregion

        #region Public Static Methods

        public static IStringLocalizer Create<T> (string cultureName = null) => Create (typeof (T).Namespace, typeof (T).Name, cultureName ?? Thread.CurrentThread.CurrentUICulture.Name);
        public static IStringLocalizer Create (Type resourceType, string cultureName = null) => Create (resourceType.Namespace, resourceType.Name, cultureName);
        public static IStringLocalizer Create (string baseName, string location, string cultureName = null) => new EmptyStringLocalizer (baseName, location, cultureName ?? Thread.CurrentThread.CurrentUICulture.Name);

        public static Localizer CreateLocalizer<T> (string cultureName = null) => Create<T> (cultureName).Get;
        public static Localizer CreateLocalizer (Type resourceType, string cultureName = null) => Create (resourceType, cultureName).Get;

        #endregion

        #region IStringLocalizer Members

        public string BaseName { get; }

        public string Location { get; }

        public string CultureName { get; }

        public PluralizationRuleDelegate PluralizationRule => (count) => count > 1 ? 1 : 0;

        public LocalizedString Get (string name, int count = -1, params object[] args) => new LocalizedString (name, name, args);

        public IEnumerable<LocalizedString> List () => Enumerable.Empty<LocalizedString> ();

        #endregion
    }
}