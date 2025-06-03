using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Fixtures.Requests;
using Nameless.Mediator.Requests;

namespace Nameless.Mediator;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringRequestHandlers_WhenBuildProvider_ThenProviderShouldResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterMediatorServices(options => {
            options.Assemblies = [
                typeof(ServiceCollectionExtensionsTests).Assembly,
                //typeof(int).Assembly
            ];
            options.UseEventHandler = false; // Disable event handlers for this test
            options.UseStreamHandler = false; // Disable stream handlers for this test
        });
        var provider = services.BuildServiceProvider();

        // act & assert
        Assert.Multiple(() => {
            var requestHandlerWithResponseRequestHandler = provider.GetService<IRequestHandler<RequestWithResponse, object>>();
            Assert.That(requestHandlerWithResponseRequestHandler, Is.Not.Null);
            Assert.That(requestHandlerWithResponseRequestHandler, Is.InstanceOf<RequestWithResponseRequestHandler>());

            var requestHandlerWithoutResponseRequestHandler = provider.GetService<IRequestHandler<RequestWithoutResponse>>();
            Assert.That(requestHandlerWithoutResponseRequestHandler, Is.Not.Null);
            Assert.That(requestHandlerWithoutResponseRequestHandler, Is.InstanceOf<RequestWithoutResponseRequestHandler>());

            var numericRequestHandler = provider.GetServices<IRequestHandler<NumericRequest, double>>().ToArray();
            Assert.That(numericRequestHandler, Has.Length.EqualTo(3));
            Assert.That(numericRequestHandler, Has.One.Items.InstanceOf<SumNumericRequestHandler>());
            Assert.That(numericRequestHandler, Has.One.Items.InstanceOf<SubtractNumericRequestHandler>());
            Assert.That(numericRequestHandler, Has.One.Items.InstanceOf<MultiplyNumericRequestHandler>());

            var dayOfWeekCloseTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<DayOfWeek>, DayOfWeek>>();
            Assert.That(dayOfWeekCloseTypeRequestHandler, Is.Not.Null);
            Assert.That(dayOfWeekCloseTypeRequestHandler, Is.InstanceOf<DayOfWeekCloseTypeRequestHandler>());

            var integerCloseTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<int>, int>>();
            Assert.That(integerCloseTypeRequestHandler, Is.Not.Null);
            Assert.That(integerCloseTypeRequestHandler, Is.InstanceOf<IntegerCloseTypeRequestHandler>());

            var structResultOpenTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<StructResult>, StructResult>>();
            Assert.That(structResultOpenTypeRequestHandler, Is.Not.Null);
            Assert.That(structResultOpenTypeRequestHandler, Is.InstanceOf<OpenGenericRequestHandler<StructResult>>());
        });
    }
}