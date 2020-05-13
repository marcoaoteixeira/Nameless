using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.WebApplication {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static TSettings ConfigureSettings<TSettings> (this IServiceCollection self, IConfiguration configuration, Func<TSettings> settingsProvider) where TSettings : class {
            if (self == null) { return default; }

            Prevent.ParameterNull (configuration, nameof (configuration));
            Prevent.ParameterNull (settingsProvider, nameof (settingsProvider));
            
            var settings = settingsProvider ();
            configuration.Bind (settings);
            self.AddSingleton (settings);
            return settings;
        }

        public static TSettings ConfigureSettings<TSettings> (this IServiceCollection self, IConfiguration configuration) where TSettings : class, new() {
            if (self == null) { return default; }

            Prevent.ParameterNull (configuration, nameof (configuration));

            var settings = new TSettings ();
            configuration.Bind (settings);
            self.AddSingleton (settings);
            return settings;
        }

        #endregion
    }
}