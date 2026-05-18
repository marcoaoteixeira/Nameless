using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Http.Endpoints.Attributes.Filtering;

/// <summary>
///     Registers an <see cref="IEndpointFilter"/> on an endpoint or group.
/// </summary>
/// <remarks>
///     Multiple <see cref="UseFilterAttribute"/> instances may be applied
///     to the same endpoint or group class. The source generator emits an
///     <c>AddEndpointFilter&lt;TFilter&gt;()</c> call for each attribute,
///     in the order they are declared.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class UseFilterAttribute : Attribute {
    /// <summary>
    ///     Gets the CLR type of the <see cref="IEndpointFilter"/>
    ///     implementation to register.
    /// </summary>
    public Type FilterType { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="UseFilterAttribute"/>
    ///     with the specified filter type.
    /// </summary>
    /// <param name="filterType">
    ///     The CLR type of the <see cref="IEndpointFilter"/> implementation.
    ///     Must not be <see langword="null"/>.
    /// </param>
    public UseFilterAttribute(Type filterType) {
        FilterType = filterType;
    }
}

/// <summary>
///     Registers an <see cref="IEndpointFilter"/> of type
///     <typeparamref name="T"/> on an endpoint or group.
/// </summary>
/// <typeparam name="T">
///     The <see cref="IEndpointFilter"/> implementation to register.
/// </typeparam>
public sealed class UseFilterAttribute<T> : UseFilterAttribute where T : IEndpointFilter {
    /// <summary>
    ///     Initializes a new instance of <see cref="UseFilterAttribute{T}"/>.
    /// </summary>
    public UseFilterAttribute() : base(typeof(T)) { }
}
