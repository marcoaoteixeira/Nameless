using Autofac;
using Nameless.Autofac;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless.DependencyInjection {
    public sealed class CoreModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(SystemClock.Instance)
                .As<IClock>()
                .SingleInstance();

            builder
                .RegisterInstance(XmlSchemaValidator.Instance)
                .As<IXmlSchemaValidator>()
                .SingleInstance();

            builder
                .RegisterInstance(PluralizationRuleProvider.Instance)
                .As<IPluralizationRuleProvider>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers the Core module.
        /// The Core module will registers these following services:
        /// <list type="table">
        ///     <item>
        ///         <term>Service <see cref="IClock"/></term>
        ///         <description>Implemented by <see cref="SystemClock"/> (Singleton)</description>
        ///     </item>
        ///     <item>
        ///         <term>Service <see cref="IXmlSchemaValidator"/></term>
        ///         <description>Implemented by <see cref="XmlSchemaValidator"/> (Singleton)</description>
        ///     </item>
        ///     <item>
        ///         <term>Service <see cref="IPluralizationRuleProvider"/></term>
        ///         <description>Implemented by <see cref="PluralizationRuleProvider"/> (Singleton)</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="self">The container builder.</param>
        /// <returns>The (<paramref name="self"/>) container builder.</returns>
        public static ContainerBuilder RegisterCoreModule(this ContainerBuilder self) {
            self.RegisterModule<CoreModule>();

            return self;
        }

        #endregion
    }
}
