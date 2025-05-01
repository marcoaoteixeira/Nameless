using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Search.Lucene.Options;

namespace Nameless.Search.Lucene;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterSearchServices(this IServiceCollection self, Action<LuceneOptions> configure, params Assembly[] supportAssemblies)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterServices(supportAssemblies);

    public static IServiceCollection RegisterSearchServices(this IServiceCollection self, IConfigurationSection luceneConfigSection, params Assembly[] supportAssemblies)
        => Prevent.Argument
                  .Null(self)
                  .Configure<LuceneOptions>(luceneConfigSection)
                  .RegisterServices(supportAssemblies);

    private static IServiceCollection RegisterServices(this IServiceCollection self, Assembly[] supportAssemblies) {
        var analyzerSelectors = supportAssemblies.SelectMany(assembly => assembly.SearchForImplementations<IAnalyzerSelector>())
                                                 .ToArray();

        foreach (var analyzerSelector in analyzerSelectors) {
            self.AddSingleton(typeof(IAnalyzerSelector), analyzerSelector);
        }

        return self.AddSingleton<IAnalyzerProvider, AnalyzerProvider>()
                   .AddSingleton<IIndexProvider, IndexProvider>();
    }
}