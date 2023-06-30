using Autofac;
using Autofac_Parameter = Autofac.Core.Parameter;

namespace Nameless.Autofac {
    /// <summary>
    /// <see cref="ContainerBuilder"/> extension methods.
    /// </summary>
    public static class ContainerBuilderExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers an implementation for a specified service. Knows how to deal with classes marked with <see cref="SingletonAttribute"/>.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <param name="lifetimeScope"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static ContainerBuilder Register<TService, TImplementation>(this ContainerBuilder self, string? name = null, LifetimeScopeType lifetimeScope = LifetimeScopeType.Transient, params Autofac_Parameter[] parameters)
            where TService : class
            where TImplementation : TService {
            return Register(
                self: self,
                service: typeof(TService),
                implementation: typeof(TImplementation),
                name: name,
                lifetimeScope: lifetimeScope,
                parameters: parameters
            );
        }

        /// <summary>
        /// Registers an implementation for a specified service. Knows how to deal with classes marked with <see cref="SingletonAttribute"/>.
        /// </summary>
        public static ContainerBuilder Register(this ContainerBuilder self, Type service, Type implementation, string? name = null, LifetimeScopeType lifetimeScope = LifetimeScopeType.Transient, params Autofac_Parameter[] parameters) {
            if (!service.IsAssignableFrom(implementation)) {
                throw new ArgumentException($"The specified type ({service}) must be assignable to {implementation}");
            }

            if (RegisterAsSingleton(self, service, implementation, name)) {
                return self;
            }

            var registration = self.RegisterType(implementation);

            registration = !string.IsNullOrWhiteSpace(name)
                    ? registration.Named(name, service)
                    : registration.As(service);

            if (!parameters.IsNullOrEmpty()) {
                registration.WithParameters(parameters);
            }

            registration.SetLifetimeScope(lifetimeScope);

            return self;
        }

        public static IRegistrationDecoratorBuilder<TService, TImplementation> RegisterTypeWithDecorator<TService, TImplementation>(this ContainerBuilder self)
            where TService : notnull
            where TImplementation : notnull, TService {
            Garda.Prevent.Null(self, nameof(self));

            return new RegistrationDecoratorBuilder<TService, TImplementation>(self);
        }

        #endregion

        #region Private Static Methods

        private static bool RegisterAsSingleton(ContainerBuilder builder, Type service, Type implementation, string? name = null) {
            var result = false;

            if (implementation.IsSingleton()) {
                var instance = SingletonAttribute.GetInstance(implementation);
                if (instance == null) {
                    throw new MemberAccessException("Couldn't access singleton accessor property.");
                }

                var registration = builder.RegisterInstance(instance);
                registration = !string.IsNullOrWhiteSpace(name)
                    ? registration.Named(name, service)
                    : registration.As(service);

                registration.SingleInstance();

                result = true;
            }

            return result;
        }

        #endregion
    }
}
