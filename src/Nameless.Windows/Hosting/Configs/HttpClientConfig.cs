using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Application;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     HTTP Client Configuration
/// </summary>
public static class HttpClientConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the HTTP client defaults.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureHttpClientDefaults(WinHostSettings settings) {
            if (settings.DisableHttpClientDefaults) { return self; }

            self.ConfigureServices(services => services.ConfigureHttpClientDefaults(
                settings.ConfigureHttpClientDefaults ?? ConfigureDefaults
            ));

            return self;

            static void ConfigureDefaults(IHttpClientBuilder builder) {
                builder.ConfigureHttpClient((provider, client) => {
                    var applicationContext = provider.GetRequiredService<IApplicationContext>();
                    var productHeaderValue = new ProductHeaderValue(applicationContext.ApplicationName, applicationContext.Version);
                    var userAgent = new ProductInfoHeaderValue(productHeaderValue);

                    client.DefaultRequestHeaders.UserAgent.Add(userAgent);
                });
            }
        }
    }
}
