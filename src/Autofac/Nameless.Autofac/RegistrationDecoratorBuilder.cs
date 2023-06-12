using Autofac;
using Autofac.Builder;
using Autofac.Features.LightweightAdapters;

namespace Nameless.Autofac {

    public sealed class RegistrationDecoratorBuilder<TService, TImplementation> : IRegistrationDecoratorBuilder<TService, TImplementation>
        where TService : notnull
        where TImplementation : notnull, TService {

        #region Private Read-Only Fields

        private readonly ContainerBuilder _builder;

        #endregion

        #region Private Properties

        private IList<Type> Decorators { get; } = new List<Type>();

        #endregion

        #region Public Constructors

        public RegistrationDecoratorBuilder(ContainerBuilder builder) {
            Prevent.Null(builder, nameof(builder));

            _builder = builder;
        }

        #endregion

        #region Private Methods

        private string StartRegistration() {
            var registrationName = typeof(TImplementation).FullName!;

            _builder
                .RegisterType<TImplementation>()
                .Named<TService>(registrationName);

            return registrationName;
        }

        private string RegisterDecorator(string registrationName, Type decoratorType) {
            var decoratorFullName = decoratorType.FullName!;

            _builder
                .RegisterType(decoratorType)
                .Keyed(decoratorFullName, decoratorType);

            _builder.RegisterDecorator<TService>(
                decorator: (ctx, inner) => (TService)ctx.ResolveKeyed(
                    serviceKey: decoratorFullName,
                    serviceType: decoratorType,
                    parameters: new TypedParameter(typeof(TService), inner)
                ),
                fromKey: registrationName,
                toKey: decoratorFullName
            );

            registrationName = decoratorFullName;

            return registrationName;
        }

        #endregion

        #region IRegistrationDecoratorBuilder<TService, TImplementation> Members

        public IRegistrationDecoratorBuilder<TService, TImplementation> WithDecorator<TDecorator>()
            where TDecorator : TService {
            Decorators.Add(typeof(TDecorator));
            return this;
        }

        public IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle> Complete() {
            var registrationName = StartRegistration();

            registrationName = Decorators
                .Take(Decorators.Count - 1)
                .Aggregate(
                    seed: registrationName,
                    func: RegisterDecorator
                );

            var lastDecorator = Decorators.Last();

            _builder
                .RegisterType(lastDecorator)
                .Keyed(
                    serviceKey: lastDecorator.Name,
                    serviceType: lastDecorator
                );

            return _builder.RegisterDecorator<TService>(
                (context, inner) => (TService)context.ResolveKeyed(
                    lastDecorator.FullName!,
                    lastDecorator,
                    new TypedParameter(typeof(TService), inner)),
                registrationName);
        }

        #endregion
    }
}
