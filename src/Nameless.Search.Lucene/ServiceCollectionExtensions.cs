using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Search.Lucene;

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterSearchServices(this IServiceCollection self, Action<LuceneOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new LuceneOptions();

        innerConfigure(options);

        return self.Configure(innerConfigure)
                   .RegisterAnalyzerSelectors(options)
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterAnalyzerSelectors(this IServiceCollection self, LuceneOptions options) {
        var analyzerSelectors = options.UseAutoRegistration
            ? options.Assemblies.GetImplementations([typeof(IAnalyzerSelector)])
            : options.AnalyzerSelectors;

        foreach (var analyzerSelector in analyzerSelectors) {
            self.AddScoped(typeof(IAnalyzerSelector), analyzerSelector);
        }

        return self;
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddScoped<IAnalyzerProvider, AnalyzerProvider>()
                   .AddScoped<IIndexProvider, IndexProvider>();
    }
}