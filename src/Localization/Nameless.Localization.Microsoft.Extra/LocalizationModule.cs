using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.Autofac;
using Nameless.Localization.Json;
using MS_IStringLocalizer = Microsoft.Extensions.Localization.IStringLocalizer;
using MS_IStringLocalizerFactory = Microsoft.Extensions.Localization.IStringLocalizerFactory;

namespace Nameless.Localization.Microsoft {

    public sealed class LocalizationModule : ModuleBase {

        #region Private Constants

        private const string TRANSLATION_PROVIDER_KEY = "be4e54c9-946e-4a42-b246-b4df5b955c93";
        private const string PLURALIZATION_RULE_PROVIDER_KEY = "10913e35-0bcc-40a8-b1fa-73329112a79f";
        private const string CULTURE_CONTEXT_KEY = "8b53a609-5c3e-4b22-9e8c-a1711bc8daeb";

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
               .Register<ICultureContext, CultureContext>(
                   name: CULTURE_CONTEXT_KEY,
                   lifetimeScope: LifetimeScopeType.Singleton
               );

            builder
                .Register<IPluralizationRuleProvider, PluralizationRuleProvider>(
                    name: PLURALIZATION_RULE_PROVIDER_KEY,
                    lifetimeScope: LifetimeScopeType.Singleton
                );

            builder
                 .Register<ITranslationProvider, TranslationProvider>(
                     name: TRANSLATION_PROVIDER_KEY,
                     lifetimeScope: LifetimeScopeType.Singleton
                 );

            builder
                .Register<IStringLocalizerFactory, StringLocalizerFactory>(
                    lifetimeScope: LifetimeScopeType.Singleton,
                    parameters: new[] {
                        ResolvedParameter.ForNamed<ICultureContext>(CULTURE_CONTEXT_KEY),
                        ResolvedParameter.ForNamed<IPluralizationRuleProvider>(PLURALIZATION_RULE_PROVIDER_KEY),
                        ResolvedParameter.ForNamed<ITranslationProvider>(TRANSLATION_PROVIDER_KEY)
                    }
                );

            builder
                .Register<MS_IStringLocalizerFactory, StringLocalizerFactoryAdapter>(
                    lifetimeScope: LifetimeScopeType.Singleton
                );

            base.Load(builder);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(IStringLocalizer),
                    factory: StringLocalizer_Resolve_Delegate
                ));
            };

            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolveMiddleware(
                    serviceType: typeof(MS_IStringLocalizer),
                    factory: MS_StringLocalizer_Resolve_Delegate
                ));
            };

            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static IStringLocalizer StringLocalizer_Resolve_Delegate(MemberInfo member, IComponentContext context)
            => context.Resolve<IStringLocalizerFactory>().Create(member.DeclaringType!);

        private static MS_IStringLocalizer MS_StringLocalizer_Resolve_Delegate(MemberInfo member, IComponentContext context)
            => context.Resolve<MS_IStringLocalizerFactory>().Create(member.DeclaringType!);

        #endregion
    }
}
