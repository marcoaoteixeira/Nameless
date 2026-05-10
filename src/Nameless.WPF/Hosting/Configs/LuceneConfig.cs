using Nameless.Lucene;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Lucene Configuration
/// </summary>
public static class LuceneConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Lucene services.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterLucene(WinHostSettings settings) {
            if (settings.DisableLucene) { return self;}

            self.ConfigureServices((ctx, services) => {
                services.RegisterLucene(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureLuceneRegistration,
                        settings.Assemblies
                    ),
                    ctx.Configuration
                );
            });

            return self;
        }
    }
}
