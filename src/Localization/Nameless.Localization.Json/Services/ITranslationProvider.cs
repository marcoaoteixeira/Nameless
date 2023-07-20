using System.Globalization;
using Nameless.Localization.Json.Objects.Translation;

namespace Nameless.Localization.Json.Services {
    public interface ITranslationProvider {
        #region Methods

        Task<Trunk> GetAsync(CultureInfo culture, CancellationToken cancellationToken = default);

        #endregion
    }
}
