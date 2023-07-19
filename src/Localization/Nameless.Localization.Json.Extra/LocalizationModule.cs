using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;

namespace Nameless.Localization.Json {
    public sealed class LocalizationModule : ModuleBase {
        #region Private Constants

        private const string CULTURE_CONTEXT_TOKEN = "CultureContext.5bd5f404-df4b-46d9-9ad4-298b765c5949";
        private const string PLURALIZATION_RULE_PROVIDER_TOKEN = "PluralizationRuleProvider.7dbbb454-e130-4b5a-8891-285596c51bd7";
        private const string TRANSLATION_PROVIDER_TOKEN = "TranslationProvider.66faa31b-c30b-4efc-9e0c-016b5c8a1e17";

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder) {
            builder
               .RegisterType<CultureContext>()
               .Named<ICultureContext>(CULTURE_CONTEXT_TOKEN)
               .SingleInstance();

            builder
               .RegisterType<PluralizationRuleProvider>()
               .Named<IPluralizationRuleProvider>(PLURALIZATION_RULE_PROVIDER_TOKEN)
               .SingleInstance();

            builder
               .RegisterType<TranslationProvider>()
               .Named<ITranslationProvider>(TRANSLATION_PROVIDER_TOKEN)
               .SingleInstance();

            builder
                .Register<IStringLocalizerFactory, StringLocalizerFactory>(
                    lifetimeScope: LifetimeScopeType.Singleton,
                    parameters: new[] {
                        ResolvedParameter.ForNamed<ICultureContext>(CULTURE_CONTEXT_TOKEN),
                        ResolvedParameter.ForNamed<IPluralizationRuleProvider>(PLURALIZATION_RULE_PROVIDER_TOKEN),
                        ResolvedParameter.ForNamed<ITranslationProvider>(TRANSLATION_PROVIDER_TOKEN)
                    }
                );

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
    }
}
