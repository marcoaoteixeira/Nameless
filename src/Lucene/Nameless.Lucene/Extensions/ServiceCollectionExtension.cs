using Microsoft.Extensions.DependencyInjection;
using Nameless.Infrastructure;
using Nameless.Lucene.Impl;
using Nameless.Lucene.Options;

namespace Nameless.Lucene {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterLucene(this IServiceCollection self, Action<LuceneOptions>? configure = null)
            => self.AddSingleton<IIndexManager>(provider => {
                var options = provider.GetPocoOptions<LuceneOptions>();

                configure?.Invoke(options);

                return new IndexManager(
                    applicationContext: provider.GetRequiredService<IApplicationContext>(),
                    analyzerProvider: new AnalyzerProvider(
                        selectors: provider.GetService<IAnalyzerSelector[]>() ?? []
                    ),
                    logger: provider.GetLogger<Impl.Index>(),
                    options: options
                );
            });

        #endregion
    }
}
