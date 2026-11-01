using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Windows.Documents.Converters;
using Nameless.Windows.Documents.Readers;

namespace Nameless.Windows.Documents;

/// <summary>
///     Document Service Host Feature
/// </summary>
public class DocumentServiceHostFeature : HostFeature<DocumentServiceFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.TryAddSingleton<IDocumentService, DocumentService>();

        builder.Services.TryAddEnumerable(options.DocumentConverters.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IDocumentConverter),
                implementation
            )
        ));

        builder.Services.TryAddEnumerable(options.DocumentReaders.Select(
            implementation => ServiceDescriptor.Singleton(
                typeof(IDocumentReader),
                implementation
            )
        ));
    }
}
