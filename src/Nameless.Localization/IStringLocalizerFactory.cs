using System;

namespace Nameless.Localization {
    public interface IStringLocalizerFactory {
        #region Methods

        IStringLocalizer Create (Type resourceType, string cultureName = null);
        IStringLocalizer Create (string baseName, string location, string cultureName = null);

        #endregion
    }
}