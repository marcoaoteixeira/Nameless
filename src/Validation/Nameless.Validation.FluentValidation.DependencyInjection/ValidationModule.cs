using System.Reflection;
using Autofac;
using FluentValidation;
using Nameless.Autofac;
using Nameless.Validation.Abstractions;
using Nameless.Validation.FluentValidation.Impl;

namespace Nameless.Validation.FluentValidation.DependencyInjection {
    public sealed class ValidationModule : ModuleBase {
        #region Public Constructors

        public ValidationModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            AssemblyScanner.FindValidatorsInAssemblies(SupportAssemblies)
                           .ForEach(result => builder.RegisterType(result.ValidatorType)
                                                     .As(result.ValidatorType)
                                                     .As(result.InterfaceType)
                                                     .InstancePerLifetimeScope());

            builder.Register(ValidatorServiceResolver)
                   .As<IValidationService>()
                   .InstancePerLifetimeScope();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IValidationService ValidatorServiceResolver(IComponentContext ctx) {
            var validators = ctx.Resolve<IValidator[]>();
            var logger = ctx.GetLogger<ValidationService>();
            var result = new ValidationService(validators, logger);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterValidationModule(this ContainerBuilder self, params Assembly[] supportAssemblies) {
            self.RegisterModule(new ValidationModule(supportAssemblies));

            return self;
        }

        #endregion
    }
}