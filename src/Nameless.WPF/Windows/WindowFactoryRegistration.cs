using Nameless.Registration;

namespace Nameless.WPF.Windows;

public class WindowFactoryRegistration : AssemblyScanAware<WindowFactoryRegistration> {
    private readonly HashSet<Type> _windows = [];

    public IReadOnlyCollection<Type> Windows => _windows;

    public WindowFactoryRegistration RegisterWindow<TWindow>()
        where TWindow : class, IWindow {
        return RegisterWindow(typeof(TWindow));
    }

    public WindowFactoryRegistration RegisterWindow(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IWindow));

        _windows.Add(type);

        return this;
    }
}