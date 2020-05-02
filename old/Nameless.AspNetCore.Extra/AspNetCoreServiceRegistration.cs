using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.AspNetCore.Localization;
using Nameless.IoC.Autofac;
using Nameless.Localization;
using Nameless.Localization.Json;

namespace Nameless.AspNetCore {
    /// <summary>
    /// The AspNetCore service registration.
    /// </summary>
    public sealed class AspNetCoreServiceRegistration : ServiceRegistrationBase {
        #region Private Constants

        private const string PLURALIZATION_RULE_PROVIDER_KEY = "PLURALIZATION_RULE_PROVIDER";
        private const string STRING_LOCALIZER_FACTORY_KEY = "STRING_LOCALIZER_FACTORY";

        #endregion

        #region Public Constants

        public const string INJECT_PROPERTY_NAMED = "T";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets whether will use the JSON <see cref="IStringLocalizer" /> implementation.
        /// </summary>
        public bool UseJsonStringLocalizer { get; set; } = true;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<PluralizationRuleProvider> ()
                .Named<IPluralizationRuleProvider> (PLURALIZATION_RULE_PROVIDER_KEY)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<StringLocalizerFactory> ()
                .Named<IStringLocalizerFactory> (STRING_LOCALIZER_FACTORY_KEY)
                .WithParameters (new Parameter[] {
                    new ResolvedParameter (
                            (parameter, ctx) => parameter.ParameterType == typeof (IFileProvider),
                            (parameter, ctx) => ctx.Resolve<IFileProvider> ()
                        ),
                        new ResolvedParameter (
                            (parameter, ctx) => parameter.ParameterType == typeof (IPluralizationRuleProvider),
                            (parameter, ctx) => ctx.ResolveNamed<IPluralizationRuleProvider> (PLURALIZATION_RULE_PROVIDER_KEY)
                        ),
                        new ResolvedParameter (
                            (parameter, ctx) => parameter.ParameterType == typeof (LocalizationSettings),
                            (parameter, ctx) => ctx.Resolve<LocalizationSettings> ()
                        )
                })
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<StringLocalizerFactoryWrapper> ()
                .As<Microsoft.Extensions.Localization.IStringLocalizerFactory> ()
                .WithParameter (new ResolvedParameter (
                    (parameter, ctx) => parameter.ParameterType == typeof (IStringLocalizerFactory),
                    (parameter, ctx) => ctx.ResolveNamed<IStringLocalizerFactory> (STRING_LOCALIZER_FACTORY_KEY)
                ))
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<HtmlLocalizerFactoryWrapper> ()
                .As<Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizerFactory> ()
                .WithParameter (new ResolvedParameter (
                    (parameter, ctx) => parameter.ParameterType == typeof (IStringLocalizerFactory),
                    (parameter, ctx) => ctx.ResolveNamed<IStringLocalizerFactory> (STRING_LOCALIZER_FACTORY_KEY)
                ))
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterType<StringLocalizerWrapper> ()
                .As<Microsoft.Extensions.Localization.IStringLocalizer> ()
                .WithParameter (new ResolvedParameter (
                    (parameter, ctx) => parameter.ParameterType == typeof (IStringLocalizerFactory),
                    (parameter, ctx) => ctx.ResolveNamed<IStringLocalizerFactory> (STRING_LOCALIZER_FACTORY_KEY)
                ))
                //.OnPreparing (OnPreparingStringLocalizerWrapper)
                .SetLifetimeScope (LifetimeScopeType.Singleton);

            builder
                .RegisterGeneric (typeof (Microsoft.Extensions.Localization.StringLocalizer<>))
                .As (typeof (Microsoft.Extensions.Localization.IStringLocalizer<>))
                .SetLifetimeScope (LifetimeScopeType.Transient);

            base.Load (builder);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration (IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            var implementationType = registration.Activator.LimitType;

            // verify if the implementation type needs logger injection via constructor.
            var hasConstructorInjection = implementationType.GetConstructors ()
                .Any (constructor => constructor.GetParameters ()
                    .Any (parameter => typeof (Microsoft.Extensions.Localization.IStringLocalizer).IsAssignableFrom (parameter.ParameterType)));

            // if need, inject and return.
            if (hasConstructorInjection) {
                // registration.Preparing += (sender, args) => {
                //     args.Parameters = args.Parameters.Concat (new [] {
                //         new TypedParameter (
                //             type: typeof (ILogger),
                //             value: GetCachedLogger (implementationType, args.Context)
                //         )
                //     });
                // };
                return;
            }

            var property = FindUserProperty (registration.Activator.LimitType);
            if (property != null) {
                var scope = registration.Activator.LimitType;
                registration.Activated += (sender, e) => {
                    if (e.Instance.GetType () != scope) { return; }
                    var localizer = ResolveLocalizerDelegate (e.Context, scope);
                    property.SetValue (e.Instance, localizer, null);
                };
            }

            base.AttachToComponentRegistration (componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static PropertyInfo FindUserProperty (Type type) => type.GetProperty (INJECT_PROPERTY_NAMED, typeof (Localizer));

        private static Localizer ResolveLocalizerDelegate (IComponentContext context, Type scope) {
            return ResolveStringLocalizer (context, scope).Get;
        }

        private static IStringLocalizer ResolveStringLocalizer (IComponentContext context, Type scope) {
            return context.Resolve<IStringLocalizerFactory> ().Create (scope);
        }

        #endregion

        #region Private Methods

        private void OnPreparingStringLocalizerWrapper (PreparingEventArgs args) {
            var limitType = args.Component.Activator.LimitType;
        }

        #endregion
    }
}