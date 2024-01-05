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

        public static ContainerBuilder RegisterCoreModule(this ContainerBuilder self) {
            self.RegisterModule<CoreModule>();

            return self;
        }

        #endregion
    }
}
