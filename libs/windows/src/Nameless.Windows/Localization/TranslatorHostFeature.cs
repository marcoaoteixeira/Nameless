using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;

namespace Nameless.Windows.Localization;

/// <summary>
///     Translator Host Feature
/// </summary>
public class TranslatorHostFeature : HostFeature<TranslatorFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        builder.Services.ConfigureOptions<TranslatorOptions>(builder.Configuration);

        var descriptor = options.Translator is not null
            ? ServiceDescriptor.Singleton(typeof(ITranslator), options.Translator)
            : ServiceDescriptor.Singleton(NullTranslator.Instance);

        builder.Services.TryAdd(descriptor);
    }
}