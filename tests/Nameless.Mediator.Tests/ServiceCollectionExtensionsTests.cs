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
            Assert.NotNull(requestHandlerWithResponseRequestHandler);
            Assert.IsType<RequestWithResponseRequestHandler>(requestHandlerWithResponseRequestHandler);

            var requestHandlerWithoutResponseRequestHandler = provider.GetService<IRequestHandler<RequestWithoutResponse>>();
            Assert.NotNull(requestHandlerWithoutResponseRequestHandler);
            Assert.IsType<RequestWithoutResponseRequestHandler>(requestHandlerWithoutResponseRequestHandler);

            var numericRequestHandlers = provider.GetServices<IRequestHandler<NumericRequest, double>>().ToArray();
            Assert.Equal(3, numericRequestHandlers.Length);
            Assert.Contains(numericRequestHandlers, item => typeof(SumNumericRequestHandler).IsAssignableFrom(item.GetType()));
            Assert.Contains(numericRequestHandlers, item => typeof(SubtractNumericRequestHandler).IsAssignableFrom(item.GetType()));
            Assert.Contains(numericRequestHandlers, item => typeof(MultiplyNumericRequestHandler).IsAssignableFrom(item.GetType()));

            var dayOfWeekCloseTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<DayOfWeek>, DayOfWeek>>();
            Assert.NotNull(dayOfWeekCloseTypeRequestHandler);
            Assert.IsType<DayOfWeekCloseTypeRequestHandler>(dayOfWeekCloseTypeRequestHandler);

            var integerCloseTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<int>, int>>();
            Assert.NotNull(integerCloseTypeRequestHandler);
            Assert.IsType<IntegerCloseTypeRequestHandler>(integerCloseTypeRequestHandler);

            var structResultOpenTypeRequestHandler = provider.GetService<IRequestHandler<OpenGenericRequest<StructResult>, StructResult>>();
            Assert.NotNull(structResultOpenTypeRequestHandler);
            Assert.IsType<OpenGenericRequestHandler<StructResult>>(structResultOpenTypeRequestHandler);
        });
    }
}