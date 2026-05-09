using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Http.Endpoints.Attributes;

/// <summary>
///     Registers an <see cref="IEndpointFilter"/> on an endpoint.
/// </summary>
/// <remarks>
///     Multiple <see cref="FilterAttribute"/> instances may be applied
///     to the same endpoint class. The source generator emits an
///     <c>AddEndpointFilter&lt;TFilter&gt;()</c> call for each attribute,
///     in the order they are declared.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class FilterAttribute : Attribute {
    /// <summary>
    ///     Gets the CLR type of the <see cref="IEndpointFilter"/>
    ///     implementation to register.
    /// </summary>
    public Type FilterType { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="FilterAttribute"/>
    ///     with the specified filter type.
    /// </summary>
    /// <param name="filterType">
    ///     The CLR type of the <see cref="IEndpointFilter"/> implementation.
    ///     Must not be <see langword="null"/>.
    /// </param>
    public FilterAttribute(Type filterType) {
        FilterType = filterType;
    }
}

/// <summary>
///     Registers an <see cref="IEndpointFilter"/> of type
///     <typeparamref name="T"/> on an endpoint.
/// </summary>
/// <typeparam name="T">
///     The <see cref="IEndpointFilter"/> implementation to register.
/// </typeparam>
public sealed class FilterAttribute<T> : FilterAttribute where T : IEndpointFilter {
    /// <summary>
    ///     Initializes a new instance of <see cref="FilterAttribute{T}"/>.
    /// </summary>
    public FilterAttribute() : base(typeof(T)) { }
}
