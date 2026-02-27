using System.Collections;

namespace Nameless.Web.MinimalEndpoints.Infrastructure;

/// <summary>
///     Represents a collection of endpoint types.
/// </summary>
public class EndpointTypeCollection : IEnumerable<Type> {
    private readonly Type[] _endpoints;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointTypeCollection"/> class.
    /// </summary>
    /// <param name="endpoints">
    ///     A collection of endpoint types to include in the collection.
    /// </param>
    public EndpointTypeCollection(IEnumerable<Type> endpoints) {
        _endpoints = [.. endpoints.Select(ThrowOnNonEndpointType).Distinct()];
    }

    /// <inheritdoc />
    public IEnumerator<Type> GetEnumerator() {
        return ((IEnumerable<Type>)_endpoints).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    private static Type ThrowOnNonEndpointType(Type type) {
        if (!typeof(IEndpoint).IsAssignableFrom(type)) {
            throw new ArgumentException($"The current type '{type.Name}' must implement '{nameof(IEndpoint)}'.");
        }

        return type;
    }
}