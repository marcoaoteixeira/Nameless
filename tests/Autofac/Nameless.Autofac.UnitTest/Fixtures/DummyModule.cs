using System.Reflection;

namespace Nameless.Autofac.Fixtures;
public class DummyModule : ModuleBase {

    public DummyModule(params Assembly[] supportAssemblies)
        : base(supportAssemblies) { }

    public Type GetImplementation<T>()
        => SearchForImplementation<T>();

    public Type GetImplementation(Type serviceType)
        => SearchForImplementation(serviceType);

    public IEnumerable<Type> GetImplementations<T>()
        => SearchForImplementations<T>();

    public IEnumerable<Type> GetImplementations(Type serviceType)
        => SearchForImplementations(serviceType);
}
