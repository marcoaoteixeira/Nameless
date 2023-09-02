using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using FluentValidation;
using Nameless.Autofac;

namespace Nameless.FluentValidation.DependencyInjection {
    public sealed class FluentValidationModule : ModuleBase {
        #region Public Constructors

        public FluentValidationModule()
            : base(Array.Empty<Assembly>()) { }

        public FluentValidationModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            AssemblyScanner
                .FindValidatorsInAssemblies(SupportAssemblies)
                .ForEach(result => builder
                    .RegisterType(result.ValidatorType)
                    .As(result.InterfaceType)
                    .InstancePerLifetimeScope()
                );

            builder
                .RegisterType<ValidatorManager>()
                .As<IValidatorManager>()
                .SingleInstance();

            base.Load(builder);
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static IModuleRegistrar AddFluentValidation(this ContainerBuilder self, params Assembly[] supportAssemblies)
            => self.RegisterModule(new FluentValidationModule(supportAssemblies));

        #endregion
    }
}