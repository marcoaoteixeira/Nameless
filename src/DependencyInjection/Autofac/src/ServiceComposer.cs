using System;
using Autofac;
using AF_Parameter = Autofac.Core.Parameter;

namespace Nameless.DependencyInjection.Autofac {
    public sealed class ServiceComposer : IServiceComposer {
        #region Public Properties

        public ContainerBuilder Builder { get; }

        #endregion

        #region Public Constructors

        public ServiceComposer (ContainerBuilder builder = null) {
            Builder = builder ?? new ContainerBuilder ();
        }

        #endregion

        #region Private Methods

        private IServiceComposer AddService (Type serviceType, Type implementationType, LifetimeScopeType scopeType, params Parameter[] parameters) {
            var registration = Builder
                .RegisterType (implementationType)
                .As (serviceType);

            foreach (var parameter in parameters) {
                AF_Parameter af_parameter = null;

                if (parameter.Type != null) {
                    af_parameter = new TypedParameter (parameter.Type, parameter.Value);
                }

                if (!string.IsNullOrWhiteSpace (parameter.Name)) {
                    af_parameter = new NamedParameter (parameter.Name, parameter.Value);
                }

                if (af_parameter == null) { continue; }

                registration.WithParameter (af_parameter);
            }

            registration.SetLifetimeScope (scopeType);

            return this;
        }

        #endregion

        #region IServiceComposer Members

        public IServiceComposer AddPerScope (Type serviceType, Type implementationType, params Parameter[] parameters) {
            return AddService (serviceType, implementationType, LifetimeScopeType.PerScope, parameters);
        }

        public IServiceComposer AddSingleton (Type serviceType, Type implementationType, params Parameter[] parameters) {
            return AddService (serviceType, implementationType, LifetimeScopeType.Singleton, parameters);
        }

        public IServiceComposer AddTransient (Type serviceType, Type implementationType, params Parameter[] parameters) {
            return AddService (serviceType, implementationType, LifetimeScopeType.Transient, parameters);
        }

        public IServiceComposer AddModule (Type moduleType) {
            Prevent.ParameterNull (moduleType, nameof (moduleType));

            if (!typeof (ModuleBase).IsAssignableFrom (moduleType)) {
                throw new InvalidOperationException ($"Parameter {nameof (moduleType)} must implement {typeof (ModuleBase)}");
            }

            var module = Activator.CreateInstance (moduleType) as ModuleBase;
            Builder.RegisterModule (module);
            return this;
        }

        public IServiceComposer AddModule (IModule module) {
            Prevent.ParameterNull (module, nameof (module));

            if (!typeof (ModuleBase).IsAssignableFrom (module.GetType ())) {
                throw new InvalidOperationException ($"Parameter {nameof (module)} must implement {typeof (ModuleBase)}");
            }

            Builder.RegisterModule (module as ModuleBase);
            return this;
        }

        #endregion
    }
}