using Microsoft.Extensions.DependencyInjection;

namespace Nameless.WPF.DependencyInjection;

/// <summary>
///     Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions {
    /// <summary>
    ///     Creates a <see cref="ServiceDescriptor"/> for the given type.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="Type"/>.
    /// </param>
    /// <param name="service">
    ///     The service type. If <see langword="null"/>, the current type
    ///     is used.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="ServiceDescriptor"/>.
    /// </returns>
    public static ServiceDescriptor CreateServiceDescriptor(this Type self, Type? service = null) {
        var lifetime = ServiceLifetimeAttribute.GetLifetime(self, fallback: ServiceLifetime.Transient);

        return new ServiceDescriptor(service ?? self, self, lifetime);
    }
}
