using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Windows.Localization;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterLocalization(Action<LocalizationRegistration>? registration = null, IConfiguration? configuration = null) {
            if (configuration is not null) {
                self.Configure<ResourceLocalizerOptions>(configuration);
            }

            return self.InnerRegisterLocalization(registration);
        }

        private IServiceCollection InnerRegisterLocalization(Action<LocalizationRegistration>? registration) {
            var settings = ActionHelper.FromDelegate(registration);
            var implementation = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<ILocalizer>().SingleOrDefault()
                : settings.Localizer;

            if (implementation is null) {
                self.TryAddSingleton(NullLocalizer.Instance);

                return self;
            }

            self.TryAddSingleton(typeof(ILocalizer), implementation);

            return self;
        }
    }
}