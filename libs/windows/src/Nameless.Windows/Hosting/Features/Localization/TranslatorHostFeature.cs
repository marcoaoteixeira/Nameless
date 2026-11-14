using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Windows.Localization;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Hosting.Features.Localization;

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
                                   .GetSectionFor<TranslatorOptions>()
                                   .Exists();

        if (translatorType is null || (translatorType == typeof(JsonTranslator) && !sectionExists)) {
            builder.Services.TryAddSingleton(NullTranslator.Instance);

            RegisterTypedTranslator(builder);

            return;
        }

        if (translatorType == typeof(JsonTranslator)) {
            // Disk-backed, and watches RootPath for changes (#191):
            // JsonTranslator subscribes to ITranslationFileProvider.Changed
            // to hot-reload the affected culture without a restart (see
            // ADR-002).
            builder.Services.TryAddSingleton<ITranslationFileProvider, FileSystemTranslationFileProvider>();
        }

        builder.Services.TryAddSingleton(typeof(ITranslator), translatorType);

        RegisterTypedTranslator(builder);
    }

    // ITranslator<T> resolution (ILogger<T>/IStringLocalizer<T> style),
    // added next to the root registration in both branches above — the
    // NullTranslator one and the configured one — so ITranslator<T> always
    // resolves, with or without a "Translator" configuration section
    // (ADR-003).
    private static void RegisterTypedTranslator(IHostApplicationBuilder builder) {
        builder.Services.TryAddSingleton<ITranslatorFactory, TranslatorFactory>();
        builder.Services.TryAddSingleton(typeof(ITranslator<>), typeof(Translator<>));
    }
}