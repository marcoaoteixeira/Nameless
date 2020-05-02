using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Nameless.WebApplication.Web {
    public partial class StartUp {
        #region Public Methods

        public void UsingLocalization (IApplicationBuilder app) {
            var supportedCultures = new [] {
                new CultureInfo ("pt-BR"),
                new CultureInfo ("en-US")
            };
            app.UseRequestLocalization (new RequestLocalizationOptions {
                DefaultRequestCulture = new RequestCulture ("pt-BR"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
        }

        #endregion
    }
}