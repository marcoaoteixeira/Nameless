using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.WPF.DependencyInjection;

/// <summary>
///     Defines the lifetime of a service.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServiceLifetimeAttribute : Attribute {
    /// <summary>
    ///     Gets or init the lifetime type.
    /// </summary>
    public ServiceLifetime Lifetime { get; init; }

    public static ServiceLifetime GetLifetime<TType>(ServiceLifetime fallback = ServiceLifetime.Singleton) {
        return GetLifetime(typeof(TType), fallback);
    }

    public static ServiceLifetime GetLifetime(Type type, ServiceLifetime fallback = ServiceLifetime.Singleton) {
        return type.GetCustomAttribute<ServiceLifetimeAttribute>()?.Lifetime ?? fallback;
    }
}