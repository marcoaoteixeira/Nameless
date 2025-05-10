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

    [Test]
    public void WhenRegisteringMediatorEventHandlers_WhenClassImplementsMoreThanOneEventHandlerInterface_ThenResolveEventHandlersAccordingly() {
        // arrange
        var serviceCollection = CreateServiceCollection();

        // act
        ServiceCollectionExtensions.RegisterMediatorServices(serviceCollection, options => {
            options.UseAutoRegister = false;
            options.EventHandlers = [typeof(MultipleEventHandler)];
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        // assert
        var userCreatedEventHandler = serviceProvider.GetService<IEventHandler<UserCreatedEvent>>();
        var userDeletedEventHandler = serviceProvider.GetService<IEventHandler<UserDeletedEvent>>();
        var multipleEventHandler = serviceProvider.GetService<MultipleEventHandler>();

        Assert.Multiple(() => {
            Assert.That(userCreatedEventHandler, Is.Not.Null);
            Assert.That(userCreatedEventHandler, Is.InstanceOf<MultipleEventHandler>());
            
            Assert.That(userDeletedEventHandler, Is.Not.Null);
            Assert.That(userDeletedEventHandler, Is.InstanceOf<MultipleEventHandler>());

            Assert.That(multipleEventHandler, Is.Not.Null);
        });
    }
}
