using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Infrastructure;

namespace Nameless.Search.Lucene;

/// <summary>
///     <see cref="IServiceCollection"/> extensions for register search services.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string ANALYZER_PROVIDER_KEY = $"{nameof(IAnalyzerProvider)} :: 8fbb1bea-8dc4-40cc-8021-a44c4e363797";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterSearch(this IServiceCollection self, Action<SearchOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .AddKeyedSingleton<IAnalyzerProvider, AnalyzerProvider>(ANALYZER_PROVIDER_KEY)
                   .AddSingleton(ResolveIndexProvider);
    }

    private static IIndexManager ResolveIndexProvider(IServiceProvider provider) {
        var analyzerProvider = provider.GetRequiredKeyedService<IAnalyzerProvider>(ANALYZER_PROVIDER_KEY);
        var applicationContext = provider.GetRequiredService<IApplicationContext>();
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
        var options = provider.GetOptions<SearchOptions>();

        return new IndexManager(analyzerProvider, applicationContext, loggerFactory, options);
    }
}