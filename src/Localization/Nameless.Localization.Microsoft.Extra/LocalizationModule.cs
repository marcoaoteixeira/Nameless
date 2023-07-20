using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;
using Nameless.Localization.Json;
using Nameless.Localization.Json.Services;
using Nameless.Localization.Json.Services.Impl;

namespace Nameless.Localization.Microsoft {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = "CultureContext.fe8a69c2-2543-42fd-a3f9-1c6e8ae1ab66";
        private const string TRANSLATION_PROVIDER_TOKEN = "TranslationProvider.9ecf2edd-1b0d-4402-8c38-07a205f74ad6";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
               .RegisterInstance(CultureContext.Instance)
               .Named<ICultureContext>(CULTURE_CONTEXT_TOKEN)
               .SingleInstance();

            builder
               .RegisterType<FileTranslationProvider>()
               .Named<ITranslationProvider>(TRANSLATION_PROVIDER_TOKEN)
               .SingleInstance();

            builder
                .Register(StringLocalizerFactoryResolver)
                .As<IStringLocalizerFactory>()
                .SingleInstance();

            builder
                .RegisterType<StringLocalizerFactoryAdapter>()
                .As<IMSStringLocalizerFactory>()
                .SingleInstance();

            base.Load(builder);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(IStringLocalizer),
                    factory: (member, context) => member.DeclaringType != null
                        ? context.Resolve<IStringLocalizerFactory>().Create(member.DeclaringType)
                        : NullStringLocalizer.Instance
                ));
            };

            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(IMSStringLocalizer),
                    factory: (member, context) => member.DeclaringType != null
                        ? context.Resolve<IMSStringLocalizerFactory>().Create(member.DeclaringType)
                        : MSNullStringLocalizer.Instance
                ));
            };

            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static IStringLocalizerFactory StringLocalizerFactoryResolver(IComponentContext context) {
            var cultureContext = context.ResolveNamed<ICultureContext>(CULTURE_CONTEXT_TOKEN);
            var translationProvider = context.ResolveNamed<ITranslationProvider>(TRANSLATION_PROVIDER_TOKEN);
            var stringLocalizerFactory = new StringLocalizerFactory(cultureContext, translationProvider);

            return stringLocalizerFactory;
        }

        #endregion
    }
}
