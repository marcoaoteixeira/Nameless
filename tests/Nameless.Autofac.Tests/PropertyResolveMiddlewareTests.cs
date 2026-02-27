using Autofac;
using Nameless.Autofac.Fixtures;
using Nameless.Autofac.Mockers;

namespace Nameless.Autofac;

public class PropertyResolveMiddlewareTests {
    [Fact]
    public void WhenResolvingTypeWithInjectedProperty_ThenGetPropertyInjected() {
        // arrange
        const string Text = "This is a Test";
        var printServiceMocker = new PrintServiceMocker();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(printServiceMocker.Build())
               .As<IPrintService>();

        builder.RegisterType<ClassWithPrintServiceProperty>()
               .ConfigurePipeline(configure => {
                   configure.Use(new PropertyResolveMiddleware(
                       typeof(IPrintService),
                       (_, ctx) => ctx.Resolve<IPrintService>()
                       )
                   );
               });

        using var container = builder.Build();
        var classWithPrintServiceProperty = container.Resolve<ClassWithPrintServiceProperty>();

        // act
        classWithPrintServiceProperty.Write(Text);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(classWithPrintServiceProperty.PrintService);
            printServiceMocker.Verify(mock => mock.Print(Text));
        });
    }
}