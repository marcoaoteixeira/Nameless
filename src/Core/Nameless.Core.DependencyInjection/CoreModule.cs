using Autofac;
using Nameless.Autofac;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless.DependencyInjection {
    public sealed class CoreModule : ModuleBase {
        #region Public Constructors

        public CoreModule()
            : base([]) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(ClockService.Instance)
                .As<IClockService>()
                .SingleInstance();

            builder
                .RegisterInstance(XmlSchemaValidator.Instance)
                .As<IXmlSchemaValidator>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder AddCore(this ContainerBuilder self) {
            self.RegisterModule<CoreModule>();

            return self;
        }

        #endregion
    }
}
