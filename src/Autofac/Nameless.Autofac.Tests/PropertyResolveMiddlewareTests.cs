using Autofac;
using Moq;
using Nameless.Autofac.Fixtures;
using Nameless.Mockers;

namespace Nameless.Autofac;
public class PropertyResolveMiddlewareTests {
    [Test]
    public void WhenResolvingTypeWithInjectedProperty_ThenGetPropertyInjected() {
        // arrange
        const string text = "This is a Test";
        var printServiceMocker = new PrintServiceMocker();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(printServiceMocker.Build())
               .As<IPrintService>();
        builder.RegisterType<ClassWithPrintServiceProperty>()
               .ConfigurePipeline(configure => {
                   configure.Use(new PropertyResolveMiddleware(serviceType: typeof(IPrintService),
                                                               factory: (_, ctx) => ctx.Resolve<IPrintService>()));
               });
        using var container = builder.Build();
        var classWithPrintServiceProperty = container.Resolve<ClassWithPrintServiceProperty>();

        // act
        classWithPrintServiceProperty.Write(text);

        // assert
        printServiceMocker.Verify(mock => mock.Print(text), Times.Once());
    }

    public class PrintServiceMocker : MockerBase<IPrintService> {

    }
}
