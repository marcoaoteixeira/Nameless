using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;
using Nameless.Localization.Json.Impl;

namespace Nameless.Localization.Json {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = "CultureContext.5bd5f404-df4b-46d9-9ad4-298b765c5949";
        private const string TRANSLATION_PROVIDER_TOKEN = "TranslationProvider.66faa31b-c30b-4efc-9e0c-016b5c8a1e17";

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
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
