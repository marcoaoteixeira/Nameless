using Microsoft.Extensions.DependencyInjection;
using Nameless.Infrastructure;
using Nameless.Lucene.Impl;
using Nameless.Lucene.Options;

namespace Nameless.Lucene;

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterLucene(this IServiceCollection self, Action<LuceneOptions>? configure = null)
        => Prevent.Argument
                  .Null(self, nameof(self))
                  .AddSingleton<IIndexProvider>(provider => {
                      var options = provider.GetOptions<LuceneOptions>();
                      
                      configure?.Invoke(options.Value);
                      return new IndexProvider(applicationContext: provider.GetRequiredService<IApplicationContext>(),
                                              analyzerProvider: new AnalyzerProvider(
                                                  selectors: provider.GetService<IAnalyzerSelector[]>() ?? []
                                              ),
                                              logger: provider.GetLogger<Impl.Index>(),
                                              options: options.Value);
                  });
}