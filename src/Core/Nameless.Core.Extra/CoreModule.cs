using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
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
                .RegisterInstance(DefaultClock.Instance);

            builder
                .RegisterType<XmlSchemaValidator>()
                .As<IXmlSchemaValidator>()
                .InstancePerDependency();

            base.Load(builder);
        }

        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration) {
            registration.PipelineBuilding += (sender, pipeline) => {
                pipeline.Use(new PropertyResolverMiddleware(
                    serviceType: typeof(IClock),
                    factory: (member, ctx) => ctx.Resolve<IClock>()
                ));
            };
            base.AttachToComponentRegistration(componentRegistry, registration);
        }

        #endregion
    }
}
