using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Lucene;

/// <summary>
///     Lucene Host Feature
/// </summary>
public class LuceneHostFeature : HostFeature<LuceneFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.ConfigureOptions<LuceneOptions>(builder.Configuration);

        // All analyzer selectors should be resolved by the same interface
        // IAnalyzerSelector, hence using TryAddEnumerable
        builder.Services.TryAddEnumerable(options.AnalyzerSelectors.Select(
            implementation => ServiceDescriptor.Singleton(typeof(IAnalyzerSelector), implementation)
        ));

        builder.Services.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
        builder.Services.TryAddSingleton<IIndexProvider, IndexProvider>();
    }
}