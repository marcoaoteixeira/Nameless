using Nameless.Autofac.Fixtures;

namespace Nameless.Autofac;
public class ModuleBaseTest {
    [Test]
    public void When_Given_Service_Type_Then_Retrieve_Implementation() {
        // arrange
        var dummyModule = new DummyModule(typeof(ModuleBaseTest).Assembly);
        var expected = typeof(ConsolePrintService);

        // act
        var actual = dummyModule.GetImplementation(expected);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void When_Given_Service_Type_Then_Retrieve_All_Implementations() {
        // arrange
        var dummyModule = new DummyModule(typeof(ModuleBaseTest).Assembly);
        var expected = new[] {
            typeof(ConsolePrintService),
            typeof(NoPrintService)
        };

        // act
        var actual = dummyModule.GetImplementations(typeof(IPrintService));

        // assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void When_Given_Generic_Service_Type_Then_Retrieve_All_Implementations() {
        // arrange
        var dummyModule = new DummyModule(typeof(ModuleBaseTest).Assembly);
        var expected = new[] {
            typeof(ConsolePrintServiceGeneric<>),
            typeof(ConcreteConsolePrintServiceGeneric),
            typeof(OverrideConsolePrintServiceGeneric<>)
        };

        // act
        var actual = dummyModule.GetImplementations(typeof(IPrintServiceGeneric<>));

        // assert
        Assert.That(actual, Is.EquivalentTo(expected));
    }
}
