using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Microsoft.Extensions.Localization;
using Nameless.Autofac;
using Nameless.Localization.Microsoft.Json.Infrastructure;
using Nameless.Localization.Microsoft.Json.Infrastructure.Impl;

namespace Nameless.Localization.Microsoft.Json {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = "CultureContext.fe8a69c2-2543-42fd-a3f9-1c6e8ae1ab66";
        private const string TRANSLATION_MANAGER_TOKEN = "TranslationManager.9ecf2edd-1b0d-4402-8c38-07a205f74ad6";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
               .RegisterInstance(CultureContext.Instance)
               .Named<ICultureContext>(CULTURE_CONTEXT_TOKEN)
               .SingleInstance();

            builder
               .RegisterType<FileTranslationManager>()
               .Named<ITranslationManager>(TRANSLATION_MANAGER_TOKEN)
               .SingleInstance();

            builder
                .Register(StringLocalizerFactoryResolver)
                .As<IStringLocalizerFactory>()
                .SingleInstance();

            base.Load(builder);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline)
                => pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(IStringLocalizer),
                    factory: (member, context) => member.DeclaringType != null
                        ? context.Resolve<IStringLocalizerFactory>().Create(member.DeclaringType)
                        : NullStringLocalizer.Instance
                ));

            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static IStringLocalizerFactory StringLocalizerFactoryResolver(IComponentContext ctx) {
            var cultureContext = ctx.ResolveNamed<ICultureContext>(CULTURE_CONTEXT_TOKEN);
            var localizationOptions = ctx.ResolveNamed<ITranslationManager>(TRANSLATION_MANAGER_TOKEN);

            return new StringLocalizerFactory(cultureContext, localizationOptions);
        }

        #endregion
    }
}
