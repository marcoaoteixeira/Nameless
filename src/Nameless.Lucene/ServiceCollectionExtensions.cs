using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers all the services required by Lucene.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configure">
    ///     The action to configure <see cref="LuceneOptions"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterLucene(this IServiceCollection self, Action<LuceneOptions>? configure = null) {
        Guard.Against.Null(self);

        self.AddOptions<LuceneOptions>()
            .Configure(configure ?? (_ => { }));

        self.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
        self.TryAddSingleton<IIndexProvider, IndexProvider>();

        return self;
    }
}
