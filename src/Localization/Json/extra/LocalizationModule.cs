using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Nameless.DependencyInjection.Autofac;

namespace Nameless.Localization.Json {
    /// <summary>
    /// The Localization service registration.
    /// </summary>
    public sealed class LocalizationModule : ModuleBase {
        #region Public Properties

        /// <summary>
        /// Gets or sets the <see cref="IPluralizationRuleProvider"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType PluralizationRuleProviderLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IStringLocalizerFactory"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType StringLocalizerFactoryLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        /// <summary>
        /// Gets or sets the <see cref="IMessageCollectionPackageProvider"/> <see cref="LifetimeScopeType"/>.
        /// </summary>
        /// <remarks>Default is <see cref="LifetimeScopeType.Singleton"/>.</remarks>
        public LifetimeScopeType MessageCollectionPackageProviderLifetimeScope { get; set; } = LifetimeScopeType.Singleton;

        #endregion

        #region Protected Override Methods

        /// <inheritdoc/>
        protected override void Load (ContainerBuilder builder) {
            builder
                .RegisterType<PluralizationRuleProvider> ()
                .As<IPluralizationRuleProvider> ()
                .SetLifetimeScope (PluralizationRuleProviderLifetimeScope);

            builder
                .RegisterType<StringLocalizerFactory> ()
                .As<IStringLocalizerFactory> ()
                .SetLifetimeScope (StringLocalizerFactoryLifetimeScope);

            builder
                .RegisterType<MessageCollectionPackageProvider> ()
                .As<IMessageCollectionPackageProvider> ()
                .SetLifetimeScope (MessageCollectionPackageProviderLifetimeScope);

            base.Load (builder);
        }

        /// <inheritdoc/>
        protected override void AttachToComponentRegistration (IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            var limitType = registration.Activator.LimitType;
            var localizerProperty = FindLocalizerProperty (limitType);
            if (localizerProperty != null) {
                registration.Activated += (sender, e) => {
                    if (e.Instance.GetType () != limitType) { return; }
                    var localizer = ResolveLocalizerDelegate (e.Context, limitType);
                    localizerProperty.SetValue (e.Instance, localizer, null);
                };
            }
            base.AttachToComponentRegistration (componentRegistry, registration);
        }

        #endregion

        #region Private Static Methods

        private static PropertyInfo FindLocalizerProperty (Type type) {
            // Look for settable properties of type "Localizer"
            var result = type
                .GetProperties (BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select (property => new {
                    PropertyInfo = property,
                        property.PropertyType,
                        IndexParameters = property.GetIndexParameters ().ToArray (),
                        Accessors = property.GetAccessors (false)
                })
                .Where (property => property.PropertyType == typeof (Localizer)) // must be a logger
                .Where (property => property.IndexParameters.Length == 0) // must not be an indexer
                .Where (property => property.Accessors.Length != 1 || property.Accessors[0].ReturnType == typeof (void)); //must have get/set, or only set

            var prop = result.SingleOrDefault ();
            return prop?.PropertyInfo;
        }

        private static Localizer ResolveLocalizerDelegate (IComponentContext context, Type scope) {
            var stringLocalizer = context.Resolve<IStringLocalizerFactory> ().Create (scope);
            return stringLocalizer.Get;
        }

        #endregion
    }
}