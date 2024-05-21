﻿using System.Reflection;
using Autofac;
using FluentValidation;
using Nameless.Autofac;
using Nameless.FluentValidation.Impl;

namespace Nameless.FluentValidation.DependencyInjection {
    public sealed class FluentValidationModule : ModuleBase {
        #region Public Constructors

        public FluentValidationModule(Assembly[] supportAssemblies)
            : base(supportAssemblies) { }

        #endregion

        #region Protected Override Methods

        protected override void Load(ContainerBuilder builder) {
            AssemblyScanner.FindValidatorsInAssemblies(SupportAssemblies)
                           .ForEach(result => builder.RegisterType(result.ValidatorType)
                                                     .As(result.ValidatorType)
                                                     .As(result.InterfaceType)
                                                     .InstancePerLifetimeScope());

            builder.Register(ValidatorManagerResolver)
                   .As<IValidationService>()
                   .InstancePerLifetimeScope();

            base.Load(builder);
        }

        #endregion

        #region Private Static Methods

        private static IValidationService ValidatorManagerResolver(IComponentContext ctx) {
            var validators = ctx.Resolve<IValidator[]>();
            var logger = ctx.GetLogger<ValidationService>();
            var result = new ValidationService(validators, logger);

            return result;
        }

        #endregion
    }

    public static class ContainerBuilderExtension {
        #region Public Static Methods

        public static ContainerBuilder RegisterFluentValidationModule(this ContainerBuilder self, params Assembly[] supportAssemblies) {
            self.RegisterModule(new FluentValidationModule(supportAssemblies));

            return self;
        }

        #endregion
    }
}