﻿using Autofac;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Nameless.Autofac;
using Nameless.Localization.Microsoft.Json.Infrastructure;
using Nameless.Localization.Microsoft.Json.Infrastructure.Impl;
using Nameless.Localization.Microsoft.Json.Options;

namespace Nameless.Localization.Microsoft.Json.DependencyInjection {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = $"{nameof(CultureContext)}::ecab0589-3491-4404-945a-d65f290e6e56";
        private const string TRANSLATION_MANAGER_TOKEN = $"{nameof(TranslationManager)}::641b294e-533e-4156-a1ce-92662c2ffcf8";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(CultureContext.Instance)
                .Named<ICultureContext>(CULTURE_CONTEXT_TOKEN)
                .SingleInstance();

            builder
                .Register(TranslationManagerResolver)
                .Named<ITranslationManager>(TRANSLATION_MANAGER_TOKEN)
                .SingleInstance();

            builder
                .Register(StringLocalizerFactoryResolver)
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

        private static ITranslationManager TranslationManagerResolver(IComponentContext ctx) {
            var options = ctx.GetPocoOptions<LocalizationOptions>();
            var fileProvider = ctx.Resolve<IFileProvider>();
            var result = new TranslationManager(fileProvider, options);

            return result;
        }

        private static IStringLocalizerFactory StringLocalizerFactoryResolver(IComponentContext ctx) {
            var cultureContext = ctx.ResolveNamed<ICultureContext>(CULTURE_CONTEXT_TOKEN);
            var translationManager = ctx.ResolveNamed<ITranslationManager>(TRANSLATION_MANAGER_TOKEN);
            var result = new StringLocalizerFactory(cultureContext, translationManager);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterLocalizationModule(this ContainerBuilder self) {
            self.RegisterModule<LocalizationModule>();

            return self;
        }

        #endregion
    }
}
