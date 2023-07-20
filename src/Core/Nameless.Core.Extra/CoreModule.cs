using Autofac;
using Nameless.Autofac;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless {
    public sealed class CoreModule : ModuleBase {
        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            builder
                .RegisterInstance(NullApplicationContext.Instance);

            builder
                .RegisterInstance(ClockService.Instance);

            builder
                .RegisterType<XmlSchemaValidator>()
                .As<IXmlSchemaValidator>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }
}
