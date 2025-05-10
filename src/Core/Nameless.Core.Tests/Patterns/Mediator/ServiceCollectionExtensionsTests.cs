using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Patterns.Mediator.Events;
using Nameless.Patterns.Mediator.Fixtures;
using Nameless.Patterns.Mediator.Fixtures.Events;

namespace Nameless.Patterns.Mediator;

public class ServiceCollectionExtensionsTests {
    private static IServiceCollection CreateServiceCollection() {
        var objectReceiver = Mock.Of<ObjectReceiver>();
        var serviceCollection = new ServiceCollection();

        return serviceCollection.AddTransient(_ => objectReceiver);
    }

    [Test]
    public void WhenRegisteringMediatorServices_ThenResolveServicesAccordingly() {
        // arrange
        var serviceCollection = CreateServiceCollection();

        // act
        ServiceCollectionExtensions.RegisterMediatorServices(serviceCollection, options => {
            options.SupportAssemblies = [typeof(ServiceCollectionExtensionsTests).Assembly];
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // assert
        var eventHandler = serviceProvider.GetRequiredService<IEventHandler<UserCreatedEvent>>();

        Assert.Multiple(() => {
            Assert.That(eventHandler, Is.Not.Null);
        });
    }

    [Test]
    public void WhenRegisteringMediatorEventHandlers_ThenResolveEventHandlerByInterface() {
        // arrange
        var serviceCollection = CreateServiceCollection();

        // act
        ServiceCollectionExtensions.RegisterMediatorServices(serviceCollection, options => {
            options.UseAutoRegister = false;
            options.EventHandlers = [typeof(UserCreatedEventHandler)];
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // assert
        var eventHandler = serviceProvider.GetRequiredService<IEventHandler<UserCreatedEvent>>();

        Assert.Multiple(() => {
            Assert.That(eventHandler, Is.Not.Null);
        });
    }

    [TestCase(typeof(IEventHandler<UserCreatedEvent>))]
    [TestCase(typeof(IEventHandler<UserDeletedEvent>))]
    public void WhenRegisteringMediatorEventHandlers_WhenClassImplementsMoreThanOneEventHandlerInterface_ThenResolveEventHandlersAccordingly(Type requestHandlerType) {
        // arrange
        var serviceCollection = CreateServiceCollection();

        // act
        ServiceCollectionExtensions.RegisterMediatorServices(serviceCollection, options => {
            options.UseAutoRegister = false;
            options.EventHandlers = [typeof(MultipleEventHandler)];
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // assert
        var requestHandler = serviceProvider.GetService(requestHandlerType);

        Assert.Multiple(() => {
            Assert.That(requestHandler, Is.Not.Null);
            Assert.That(requestHandler, Is.InstanceOf<MultipleEventHandler>());
        });
    }
}
