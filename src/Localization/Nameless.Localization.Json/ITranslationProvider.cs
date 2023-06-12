using System.Globalization;
using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {

    public interface ITranslationProvider {

        #region Methods

        Task<Translation> GetAsync(CultureInfo culture, CancellationToken cancellationToken = default);

        #endregion
    }
}
