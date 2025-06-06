using System.Reflection;
using Autofac;
using Moq;
using Nameless.Autofac.Fixtures;
using Nameless.Autofac.Mockers;

namespace Nameless.Autofac;

public class PropertyResolveMiddlewareTests {
    [Fact]
    public void WhenResolvingTypeWithInjectedProperty_ThenGetPropertyInjected() {
        // arrange
        const string text = "This is a Test";
        var printServiceMocker = new PrintServiceMocker();
        var builder = new ContainerBuilder();
        builder.RegisterInstance(printServiceMocker.Build())
               .As<IPrintService>();

        var resolver = (MemberInfo _, IComponentContext ctx) => ctx.Resolve<IPrintService>();

        builder.RegisterType<ClassWithPrintServiceProperty>()
               .ConfigurePipeline(configure => {
                   configure.Use(new PropertyResolveMiddleware(typeof(IPrintService), resolver));
               });

        using var container = builder.Build();
        var classWithPrintServiceProperty = container.Resolve<ClassWithPrintServiceProperty>();

        // act
        classWithPrintServiceProperty.Write(text);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(classWithPrintServiceProperty.PrintService);
            printServiceMocker.Verify(mock => mock.Print(text), Times.Once());
        });
    }
}