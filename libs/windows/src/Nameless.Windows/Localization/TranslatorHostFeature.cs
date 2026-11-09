using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Localization;

/// <summary>
///     Translator Host Feature
/// </summary>
public class TranslatorHostFeature : HostFeature<TranslatorFeatureOptions> {
    /// <inheritdoc/>
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        builder.Services.ConfigureOptions<TranslatorOptions>(builder.Configuration);

        var options = ActionHelper.FromDelegate(ConfigureFeature)
                                  .WithAssemblies(context.Assemblies);

        var translatorType = options.Translator;

        // No "Translator" configuration section at all means the host
        // never opted into JsonTranslator's disk-backed lookups. Registering
        // it anyway would make resolving ITranslator throw as soon as
        // FileSystemTranslationFileProvider needs a RootPath that was never
        // configured. Fall back to NullTranslator instead — AC2. A custom,
        // explicitly-registered translator (via WithTranslator<T>()) isn't
        // necessarily tied to that section, so it is registered as-is.
        var sectionExists = builder.Configuration
            .GetSectionFor<TranslatorOptions>(ConfigurationSectionNameAttribute.GetSectionName<TranslatorOptions>())
            .Exists();

        if (translatorType is null || (translatorType == typeof(JsonTranslator) && !sectionExists)) {
            builder.Services.TryAdd(ServiceDescriptor.Singleton(NullTranslator.Instance));

            return;
        }

        if (translatorType == typeof(JsonTranslator)) {
            // Disk-backed, and watches RootPath for changes (#191):
            // JsonTranslator subscribes to ITranslationFileProvider.Changed
            // to hot-reload the affected culture without a restart (see
            // ADR-002).
            builder.Services.TryAddSingleton<ITranslationFileProvider, FileSystemTranslationFileProvider>();
        }

        builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ITranslator), translatorType));
    }
}