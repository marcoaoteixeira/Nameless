using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AspNetCore.Localization {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection AddNamelessLocalization (this IServiceCollection self) {
            if (self == null) { return null; }

            self.AddSingleton (typeof (Microsoft.Extensions.Localization.IStringLocalizerFactory), typeof (StringLocalizerFactory));
            self.AddTransient (typeof (Microsoft.Extensions.Localization.IStringLocalizer<>), typeof (Microsoft.Extensions.Localization.StringLocalizer<>));
            self.AddTransient (typeof (Microsoft.Extensions.Localization.IStringLocalizer), typeof (StringLocalizer));

            self.AddSingleton (typeof (Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory), typeof (HtmlLocalizerFactory));
            self.AddTransient (typeof (Microsoft.AspNetCore.Mvc.Localization.IViewLocalizer), typeof (ViewLocalizer));

            return self;
        }

        #endregion
    }
}