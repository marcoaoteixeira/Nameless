using Nameless.Registration;

namespace Nameless.WPF.Windows;

public class WindowFactoryRegistrationSettings : AssemblyScanAware<WindowFactoryRegistrationSettings> {
    private readonly HashSet<Type> _windows = [];

    public IReadOnlyCollection<Type> Windows => _windows;

    public WindowFactoryRegistrationSettings RegisterWindow<TWindow>()
        where TWindow : class, IWindow {
        return RegisterWindow(typeof(TWindow));
    }

    public WindowFactoryRegistrationSettings RegisterWindow(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNotAssignableFrom(type, typeof(IWindow));

        _windows.Add(type);

        return this;
    }
}