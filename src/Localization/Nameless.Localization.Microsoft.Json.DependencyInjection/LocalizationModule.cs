using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Nameless.Autofac;
using Nameless.Localization.Microsoft.Json.Infrastructure;
using Nameless.Localization.Microsoft.Json.Infrastructure.Impl;
using CoreRoot = Nameless.Root;

namespace Nameless.Localization.Microsoft.Json.DependencyInjection {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = $"{nameof(CultureContext)}::ecab0589-3491-4404-945a-d65f290e6e56";
        private const string TRANSLATION_MANAGER_TOKEN = $"{nameof(FileTranslationManager)}::641b294e-533e-4156-a1ce-92662c2ffcf8";
        private const string FILE_PROVIDER_TOKEN = $"{nameof(PhysicalFileProvider)}::672d2bfd-5fd9-46e6-bc5a-54245ab95d4d";

        #endregion

        #region Public Constructors

        public LocalizationModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(CultureContext.Instance)
                .Named<ICultureContext>(CULTURE_CONTEXT_TOKEN)
                .SingleInstance();

            builder
                .Register(RegisterFileProvider)
                .Named<IFileProvider>(FILE_PROVIDER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveTranslationManager)
                .Named<ITranslationManager>(TRANSLATION_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(ResolveStringLocalizerFactory)
                .As<IStringLocalizerFactory>()
                .SingleInstance();

            builder
                .RegisterGeneric(typeof(StringLocalizer<>))
                .As(typeof(IStringLocalizer<>))
                .InstancePerDependency();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static LocalizationOptions? GetLocalizationOptions(IComponentContext ctx) {
            var configuration = ctx.ResolveOptional<IConfiguration>();
            var options = configuration?
                .GetSection(nameof(LocalizationOptions).RemoveTail(CoreRoot.Defaults.OptsSetsTails))
                .Get<LocalizationOptions>();

            return options;
        }

        private static IFileProvider RegisterFileProvider(IComponentContext ctx)
            => new PhysicalFileProvider(typeof(LocalizationModule).Assembly.GetDirectoryPath());

        private static ITranslationManager ResolveTranslationManager(IComponentContext ctx) {
            var options = GetLocalizationOptions(ctx);
            var fileProvider = ctx.ResolveNamed<IFileProvider>(FILE_PROVIDER_TOKEN);
            var result = new FileTranslationManager(fileProvider, options);

            return result;
        }

        private static IStringLocalizerFactory ResolveStringLocalizerFactory(IComponentContext ctx) {
            var cultureContext = ctx.ResolveNamed<ICultureContext>(CULTURE_CONTEXT_TOKEN);
            var translationManager = ctx.ResolveNamed<ITranslationManager>(TRANSLATION_MANAGER_TOKEN);
            var result = new StringLocalizerFactory(cultureContext, translationManager);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder AddLocalization(this ContainerBuilder self) {
            self.RegisterModule<LocalizationModule>();

            return self;
        }

        #endregion
    }
}
