using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers all the services required by Lucene.
        /// </summary>
        /// <param name="configure">
        ///     The action to configure <see cref="LuceneOptions"/>.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLucene(Action<LuceneOptions>? configure = null) {
            self.AddOptions<LuceneOptions>()
                .Configure(configure ?? (_ => { }));

            self.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
            self.TryAddSingleton<IIndexProvider, IndexProvider>();

            return self;
        }
    }
}