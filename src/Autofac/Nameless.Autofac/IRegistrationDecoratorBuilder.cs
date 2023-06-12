using Autofac.Builder;
using Autofac.Features.LightweightAdapters;

namespace Nameless.Autofac {

    public interface IRegistrationDecoratorBuilder<TService, TImplementation>
        where TService : notnull
        where TImplementation : notnull, TService {

        #region Methods

        IRegistrationDecoratorBuilder<TService, TImplementation> WithDecorator<TDecorator>()
            where TDecorator : TService;

        IRegistrationBuilder<TService, LightweightAdapterActivatorData, DynamicRegistrationStyle> Complete();

        #endregion
    }
}
