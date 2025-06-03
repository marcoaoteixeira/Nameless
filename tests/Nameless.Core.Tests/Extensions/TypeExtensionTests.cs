using Nameless.Fixtures;

namespace Nameless;

public class TypeExtensionTests {
    [TestCase(typeof(IGenericInterface<>), typeof(DeriveConcreteGenericInterfaceImpl), true)]
    [TestCase(typeof(GenericAbstractClass<>), typeof(ConcreteGenericAbstractClassImpl), true)]
    [TestCase(typeof(GenericAbstractClass<>), typeof(DeriveConcreteGenericAbstractClassImpl), true)]
    [TestCase(typeof(IGenericInterface<>), typeof(ConcreteMixedGenericImpl), true)]
    [TestCase(typeof(GenericAbstractClass<>), typeof(ConcreteMixedGenericImpl), true)]
    [TestCase(typeof(IGenericInterface<>), typeof(DeriveConcreteMixedGenericImpl), true)]
    [TestCase(typeof(GenericAbstractClass<>), typeof(DeriveConcreteMixedGenericImpl), true)]
    public void Call_IsAssignableFromGeneric(Type generic, Type concrete, bool isAssignableFrom) {
        // arrange

        // act
        var actual = generic.IsAssignableFromGenericType(concrete);

        var origin = generic.IsAssignableFrom(concrete);
        Console.WriteLine($"{generic.Name} is assignable from {concrete.Name}: {origin}");

        // assert
        Assert.That(actual, Is.EqualTo(isAssignableFrom));
    }
}