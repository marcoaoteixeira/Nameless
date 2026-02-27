using Nameless.Fixtures;

namespace Nameless;

public class TypeExtensionTests {
    [Theory]
    [InlineData(typeof(IGenericInterface<>), typeof(DeriveConcreteGenericInterfaceImpl), true)]
    [InlineData(typeof(GenericAbstractClass<>), typeof(ConcreteGenericAbstractClassImpl), true)]
    [InlineData(typeof(GenericAbstractClass<>), typeof(DeriveConcreteGenericAbstractClassImpl), true)]
    [InlineData(typeof(IGenericInterface<>), typeof(ConcreteMixedGenericImpl), true)]
    [InlineData(typeof(GenericAbstractClass<>), typeof(ConcreteMixedGenericImpl), true)]
    [InlineData(typeof(IGenericInterface<>), typeof(DeriveConcreteMixedGenericImpl), true)]
    [InlineData(typeof(GenericAbstractClass<>), typeof(DeriveConcreteMixedGenericImpl), true)]
    public void Call_IsAssignableFromGeneric(Type generic, Type concrete, bool isAssignableFrom) {
        // arrange

        // act
        var actual = generic.IsAssignableFromGeneric(concrete);

        var origin = generic.IsAssignableFrom(concrete);
        Console.WriteLine($"{generic.Name} is assignable from {concrete.Name}: {origin}");

        // assert
        Assert.Equal(isAssignableFrom, actual);
    }
}