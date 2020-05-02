using System.Collections.Generic;

namespace Nameless.Localization {
    public interface IStringLocalizer {
        #region Properties

        string BaseName { get; }
        string Location { get; }
        string CultureName { get; }
        PluralizationRuleDelegate PluralizationRule { get; }

        #endregion

        #region Methods

        LocalizedString Get (string text, int count = -1, params object[] args);
        IEnumerable<LocalizedString> List ();

        #endregion
    }
}