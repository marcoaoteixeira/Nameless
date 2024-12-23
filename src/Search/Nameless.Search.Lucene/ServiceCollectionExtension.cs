using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Search.Lucene.Options;

namespace Nameless.Search.Lucene;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddLuceneSearch(this IServiceCollection self, Action<LuceneOptions> configure, params Assembly[] supportAssemblies)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterSearchServices(supportAssemblies);

    public static IServiceCollection AddLuceneSearch(this IServiceCollection self, IConfigurationSection luceneConfigSection, params Assembly[] supportAssemblies)
        => Prevent.Argument
                  .Null(self)
                  .Configure<LuceneOptions>(luceneConfigSection)
                  .RegisterSearchServices(supportAssemblies);

    private static IServiceCollection RegisterSearchServices(this IServiceCollection self, Assembly[] supportAssemblies) {
        var analyzerSelectors = supportAssemblies.SelectMany(assembly => assembly.SearchForImplementations<IAnalyzerSelector>())
                                                 .ToArray();

        foreach (var analyzerSelector in analyzerSelectors) {
            self.AddSingleton(typeof(IAnalyzerSelector), analyzerSelector);
        }

        return self.AddSingleton<IAnalyzerProvider, AnalyzerProvider>()
                   .AddSingleton<IIndexProvider, IndexProvider>();
    }
}