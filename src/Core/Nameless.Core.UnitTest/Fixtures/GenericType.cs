namespace Nameless.Fixtures;

public interface IGenericInterface<T> { }

public class ConcreteGenericInterfaceImpl : IGenericInterface<object> { }

public class DeriveConcreteGenericInterfaceImpl : ConcreteGenericInterfaceImpl { }

public abstract class GenericAbstractClass<T> { }

public class ConcreteGenericAbstractClassImpl : GenericAbstractClass<object> { }

public class DeriveConcreteGenericAbstractClassImpl : ConcreteGenericAbstractClassImpl { }


public class ConcreteMixedGenericImpl : GenericAbstractClass<object>, IGenericInterface<object> { }

public class DeriveConcreteMixedGenericImpl : ConcreteMixedGenericImpl { }