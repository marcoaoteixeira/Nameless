using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Lucene.Options;

namespace Nameless.Lucene;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddLucene(this IServiceCollection self, Action<LuceneOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterLuceneServices();

    public static IServiceCollection AddLucene(this IServiceCollection self, IConfigurationSection luceneOptionsConfigurationSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<LuceneOptions>(luceneOptionsConfigurationSection)
                  .RegisterLuceneServices();

    private static IServiceCollection RegisterLuceneServices(this IServiceCollection self)
        => self.AddSingleton<IAnalyzerSelector[]>([])
               .AddSingleton<IAnalyzerProvider, AnalyzerProvider>()
               .AddSingleton<IIndexProvider, IndexProvider>();
}